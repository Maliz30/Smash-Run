# Smash-Run
Este repositório é destinado ao desenvolvimento do projeto prático da disciplina de Tópicos Especiais em Jogos Digitais. O foco do trabalho é o desenvolvimento de uma aplicação em XR (Realidade Estendida) utilizando Unity.

Para gerenciar assets pesados (modelos 3D, texturas e binários) sem comprometer a performance do repositório, utilizamos o Git LFS (Large File Storage).

## 🛠 Configuração do Git LFS
Siga os passos abaixo para preparar o ambiente local e gerenciar arquivos grandes (como assets, modelos 3D ou pastas compactadas). 

**Atenção:** As etapas 2 e 3 já foram configuradas no repositório remoto. Dessa forma, ao clonar o repositório apenas a **etapa 1** precisa ser realizada.

### 1. Instalação e Ativação
Instale a extensão no seu sistema e habilite-a localmente para que os arquivos grandes sejam baixados corretamente:

```Bash
# Instala o pacote no sistema (Debian/Ubuntu)
sudo apt-get install git-lfs

# Ativa o LFS no repositório local
git lfs install
```

### 2. Rastreamento de Arquivos
As extensões abaixo já estão sendo monitoradas pelo LFS. Caso precise adicionar novas (ex: `.fbx` ou `.wav`), utilize:

```Bash
# Exemplo para arquivos compactados e arquivos de Photoshop
git lfs track "*.extensao"
```

### 3. Persistência das Configurações

O arquivo .gitattributes já contém as regras de rastreamento e não deve ser removido.

Após rastrear as extensões, é necessário versionar o arquivo .gitattributes, que armazena essas regras:

``` Bash
git add .gitattributes
git commit -m "setup: configura rastreamento LFS para arquivos .zip e .psd"
```

## 🎮 Fluxo de Trabalho em Grupo (Unity + Git)
Para evitar conflitos de arquivos e perda de trabalho no desenvolvimento XR, todos os membros devem seguir estas diretrizes:

### 1. Configuração Inicial da Unity
Antes de iniciar as alterações, verifique se o projeto está configurado corretamente em `Edit > Project Settings > Editor`:

- **Version Control**: Selecione Visible Meta Files.

- **Asset Serialization**: Selecione Force Text (Isso permite que o Git "leia" as cenas e prefabs como texto, facilitando a resolução de conflitos).

### 2. Gestão de Arquivos e .gitignore

- Não remova o .gitignore: Ele já está configurado para ignorar pastas locais e temporárias (`Library/`, `Temp/`, `Logs/`).

- Arquivos .meta: Toda vez que criar, mover ou renomear um arquivo na Unity, um arquivo .meta correspondente será gerado/alterado. **Eles devem sempre ser commitados junto com o arquivo principal.** Nunca delete ou ignore os arquivos .meta. Eles contêm as referências que a Unity usa para ligar seus scripts aos objetos na cena.

### 3. Boas práticas
- Duas pessoas devem evitar editar a mesma .unity (Cena) ao mesmo tempo. Em casos onde alterações são necessárias, devem ser criadas mecânicas em Prefabs separadas.

- Sempre dê um git pull antes de começar sua sessão de trabalho e antes de dar git push.

- Crie cenas próprias para testar suas funcionalidades de XR (ex: Cena_Maria_Sandbox) e só leve para a cena principal quando estiver estável.