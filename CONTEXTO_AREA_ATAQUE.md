# Contexto: refatoração do sistema de martelos + AreaAtaque (Smash-Run)

Este arquivo resume uma conversa de trabalho sobre o projeto Unity **Smash-Run**
(`c:\Users\Usuario\Desktop\Smash-Run`), branch `teste-area-ataque`, para que o
contexto possa ser retomado em outra máquina/chat.

## Ponto de partida

Pergunta original: o objeto `AreaAtaque` da cena (script `GroundAreaFollow`)
estava apenas **seguindo a posição X/Z do martelo** (`transform.position`),
sem ler nenhum input/botão diretamente. Confirmado lendo o código e a cena
(`Assets/Scenes/CenaPrincipal.unity`): o campo `target` do `GroundAreaFollow`
apontava para o Transform do prefab do martelo (`SM_Bouncy_Hammer_Toy`).

## Objetivo pedido pelo usuário

Inverter o fluxo:
- Os martelos devem **ficar parados em um canto do mapa** (não se mover mais
  com os botões).
- Os **botões que hoje movem o martelo** devem passar a mover a `AreaAtaque`.
- O **botão de ataque do martelo** deve fazer o martelo atacar **onde a
  AreaAtaque estiver** no momento do aperto, e depois voltar para o canto.

## Arquitetura de input descoberta no projeto (antes da mudança)

O jogo é um jogo de cabine/VR (Meta/Oculus, pastas `[BuildingBlock]`,
`ISDK_...`, objeto `Cabine` com alavancas "Alavanca FT"/"Alavanca DE").
Existem **2 instâncias de `HammerController`** na cena (2 martelos, mesmo
prefab), e várias fontes de input diferentes, todas convergindo no
`HammerController`:

- `IHammerInputProvider` (interface): `GetMovementInput()`, `GetHeightInput()`,
  `GetAttackTrigger()`. Implementado por:
  - `KeyboardHammerInput` (teclas IJKL + U/O de altura + P de ataque — usado
    para teste desktop). As duas instâncias de martelo têm essa mesma
    configuração de teclas.
  - `AlavancaControle` (alavanca física VR, calcula input a partir da
    inclinação captada por `OVRInput` grip trigger).
  - `ButtonHammerInput` (botões poke direcionais tipo `DirectionalButton`,
    sem trigger de ataque).
- `HammerController.SetButtonMovement(source, dir)` — chamado diretamente
  (fora da interface) por `MovePressButton` (botão físico tipo "plunger" que
  detecta deslocamento e empurra o martelo em uma direção fixa enquanto
  pressionado).
- `HammerController.TryStartAttack()` — chamado diretamente por
  `PressButton` (outro botão físico "plunger", com cooldown, que dispara o
  ataque).

`HammerController` tem uma máquina de estados: `Idle -> WindingUp -> Attacking
-> Recovering -> Idle`. No Idle ele se movia livremente em X/Z (e altura) a
partir do input agregado. No ataque, ele subia (`windUpHeight`), descia até
`groundY` causando dano, e voltava para a posição de onde começou o wind-up
(que era sempre onde ele estava parado).

O `AreaAtaque` (`GroundAreaFollow`) só tinha essa lógica:
```csharp
position.x = target.position.x;
position.z = target.position.z;
```

## Decisões tomadas com o usuário

Havia 2 ambiguidades importantes, resolvidas via perguntas diretas:

1. **Existem 2 martelos — qual ataca?**
   Resposta do usuário: só **um dos dois** ataca por vez, escolhido pela
   posição da `AreaAtaque` em relação ao **centro do mapa**:
   - Área do lado esquerdo → martelo esquerdo ataca.
   - Área do lado direito → martelo direito ataca.
   - Área "mais para um lado" (não exatamente no centro) → o martelo desse
     lado ataca.
   - Área **exatamente no centro** → o **martelo direito** ataca (regra de
     desempate).

2. **O martelo deve teleportar instantaneamente até a Area para atacar, ou se
   deslocar suavemente?**
   Resposta do usuário: **deslocar-se suavemente** até a Area durante o
   wind-up (não é um teleporte instantâneo).

## Mudanças de código implementadas

### `Assets/PlayerScripts/GroundAreaFollow.cs` (reaproveitado, mesma classe/arquivo
para não quebrar o guid/vínculo do componente já existente na cena)
- Não segue mais nenhum `target`.
- Agora tem `moveSpeed` e limites opcionais (`limitX/minX/maxX`,
  `limitZ/minZ/maxZ`).
- Expõe `public void ApplyMovementInput(Vector3 movementInput, float deltaTime)`
  que move o próprio `transform.position` em X/Z, respeitando os limites.

### `Assets/PlayerScripts/HammerController.cs`
- Removidos: `moveSpeed`, `verticalSpeed`, limites X/Z, o método privado
  `ApplyMovement(...)` e o método morto `MoveFromButton(...)` (não eram mais
  necessários porque o martelo não se move mais livremente no Idle).
- Novo: campos `[SerializeField] private GroundAreaFollow attackArea;` e
  `[SerializeField] private HammerAttackDispatcher attackDispatcher;`.
- `Awake()` agora guarda `homePosition` (a posição inicial/canto do martelo).
- `Update()`: continua coletando o input agregado (providers + botões), mas
  em vez de aplicar em si mesmo, repassa para a Area:
  `attackArea.ApplyMovementInput(providerMovementInput, Time.deltaTime)`.
  Quando detecta `attackTriggered` (de qualquer provider, ex. tecla P),
  chama `attackDispatcher.TriggerAttack()` em vez de se autoatacar.
- `TryStartAttack()` agora é `TryStartAttack(Vector3 targetPosition)` — só
  quem decide "qual martelo ataca e para onde" é o dispatcher.
- `UpdateIdle()` não existe mais como movimento — o martelo simplesmente
  fica parado no `homePosition` (só a rotação idle continua sendo aplicada).
- `UpdateWindingUp()`: agora faz `Vector3.Lerp(windUpStartPosition,
  attackTargetPosition, t)` para o X/Z (deslocamento suave até a Area),
  mantendo a lógica de subida vertical (`windUpHeight`) já existente.
- `TransitionTo(Recovering)`: o alvo de recuperação agora é `homePosition`
  (o martelo volta para o canto), e não mais para onde ele bateu.

### `Assets/PlayerScripts/HammerAttackDispatcher.cs` (novo arquivo)
```csharp
[DisallowMultipleComponent]
public class HammerAttackDispatcher : MonoBehaviour
{
    [SerializeField] private HammerController leftHammer;
    [SerializeField] private HammerController rightHammer;
    [SerializeField] private Transform attackArea;
    [SerializeField] private float centerX = 0f;

    public bool TriggerAttack()
    {
        if (attackArea == null) return false;
        HammerController hammer = attackArea.position.x < centerX ? leftHammer : rightHammer;
        return hammer != null && hammer.TryStartAttack(attackArea.position);
    }
}
```
(`< centerX` cobre a regra de desempate: exatamente no centro cai no `else`,
ou seja, martelo direito.)

### `Assets/Cabin Assets/Scripts/PressButton.cs`
- Trocado `HammerController hammerController` / `autoFindHammer` por
  `HammerAttackDispatcher attackDispatcher` / `autoFindDispatcher`.
- `hammerController.TryStartAttack()` → `attackDispatcher.TriggerAttack()`.

### Não precisaram mudar (continuam funcionando sem alteração)
`MovePressButton.cs`, `AlavancaControle.cs`, `ButtonHammerInput.cs`,
`KeyboardHammerInput.cs` — todos continuam falando com `HammerController`
exatamente como antes; só o que o `HammerController` *faz* com esse input
mudou.

## Pendências no Editor Unity (ainda não feitas — próximos passos)

1. Criar um GameObject (ex.: "GameplayManager") com o componente
   `HammerAttackDispatcher` e arrastar:
   - `Left Hammer` / `Right Hammer` → os dois `HammerController` da cena.
   - `Attack Area` → o objeto `AreaAtaque`.
   - `Center X` → o X real do centro do mapa.
2. Em cada `HammerController` (nos dois martelos): arrastar `AreaAtaque`
   (componente `GroundAreaFollow`) no campo `Attack Area`, e o
   `HammerAttackDispatcher` criado no passo 1 no campo `Attack Dispatcher`.
3. Em cada `PressButton` de ataque da cabine: arrastar o
   `HammerAttackDispatcher` no campo que antes era `hammerController`.
4. No `GroundAreaFollow` da `AreaAtaque`: configurar `Move Speed` e os
   limites X/Z (`Limit X/Min X/Max X`, `Limit Z/Min Z/Max Z`) para o
   tamanho real do mapa.
5. **Atenção**: os dois martelos hoje têm `KeyboardHammerInput` com as
   mesmas teclas (IJKL/P). Se os dois ficarem ativos ao mesmo tempo, o
   movimento da Area vai somar em dobro (dois providers mandando o mesmo
   input). Recomenda-se desativar/remover o `KeyboardHammerInput` de um
   deles (ou de ambos, se só os controles físicos de cabine forem usados de
   verdade em produção).
6. Testar no Editor: mover a Area para os dois lados do mapa, apertar o
   botão de ataque e confirmar que o martelo correto (esquerdo/direito) se
   desloca suavemente até a Area, bate, e volta para o canto.

Nenhum teste no Editor foi feito ainda nesta conversa (o ambiente atual não
tem como rodar o Unity Editor) — a verificação funcional fica pendente.
