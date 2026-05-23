# Configuração e Build para Android

Este documento contém os passos necessários para configurar o ambiente de desenvolvimento, preparar o celular e resolver problemas comuns de compilação no Linux.

---

## 1. Configuração do Dispositivo Mobile

Antes de rodar o projeto, o celular precisa ser preparado para aceitar a instalação via Unity:

1.  **Conexão Física:** Conecte o celular ao PC via USB. No celular, selecione a opção de **"Transferência de Dados"**.
2.  **Habilitar Modo Desenvolvedor:**
    * Vá em **Configurações > Sobre o telefone > Informações do Software**.
    * Toque repetidamente (cerca de 7 vezes) no campo **Número de Compilação** até que a mensagem "Você agora é um desenvolvedor" apareça.
3.  **Ativar Depuração USB:**
    * Volte ao menu principal de Configurações.
    * Acesse a nova seção **Opções do Desenvolvedor**.
    * Ative a chave **Depuração USB**. 

---

## 2. Configurações de Build no Unity

Para garantir que o app funcione e abra a cena correta:

1.  Vá em **File > Build Profiles**.
2.  **Run Device:** Na lista suspensa, selecione o seu celular (ele deve aparecer pelo nome do modelo após a configuração).
3.  **Cenas (Scenes in Build):**
    * No campo superior esquerdo, verifique as cenas listadas.
    * Adicione a cena localizada em `Scenes/CenaPrincipal` ou a que deseja testar.
    * **Importante:** Desmarque qualquer outra cena. A `CenaPrincipal` ou a que preferir deve estar com o índice 0.

---

## 3. Correção do Erro de Compilação (Linux - Clang++)

No Linux, o Unity pode falhar ao localizar o compilador do NDK. Se o erro "Clang++ not found" aparecer, siga estes passos:

1.  **Organização da Pasta:**
    * Localize onde o Unity baixou o Android NDK (caminho: ~/Unity/Hub/Editor/6000.4.0f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK/).
    * Se os arquivos estiverem largados na pasta raiz do NDK, crie uma subpasta com o nome da versão (ex: `android-ndk-r27c`) e mova todos os arquivos para dentro dela.
2.  **Apontamento Manual:**
    * No Unity, vá em **Edit > Preferences > External Tools**.
    * Desmarque a opção "Android NDK Installed with Unity".
    * No campo de caminho, aponte para a **nova pasta** que você criou (a que contém os arquivos agora organizados). Caminho: ~/Unity/Hub/Editor/6000.4.0f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK/android-ndk-r27c

---

## 4. Como Rodar o Projeto

Com tudo configurado:
1.  Pressione **Ctrl + B** (ou clique em *Build and Run*).
2.  O Unity irá compilar o projeto, gerar o APK, instalá-lo no celular e abrir o app automaticamente.
3.  Caso queira rodar o mesmo projeto novamente basta ir em configurações do celular e encontrar um aplicativo com a logo da unity.

---
