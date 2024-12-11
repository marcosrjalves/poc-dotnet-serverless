# Thanks
Special thanks to @pharindoko for making his repo public available.
[template repository](https://github.com/pharindoko/serverlessDotNetStarter)

# PT-BR

## Instalação
O repo ja esta configurado com um dev container. Todos os testes foram feitos de dentro desse dev container. A principio, tudo funcionando.

comandos:

```bash

git clone repo

cd repo

cd microservices/sandbox-dotnet

dotnet restore

```

### para buildar, de dentro da pasta do microserviço:

```bash
sudo ./build.sh

```

### para deployar: (precisa estar com as chaves do aws cli configuradas)

```bash
npm i
npx sls deploy
```


### Para adicionar novas funções:
Todas as 3 funções atuais estão todas definidas dentro do arquivo Handler.cs.
Basta acrescentar uma nova função nesse arquivo, ou criar um novo arquivo e referencia-lo no serverless.yml
