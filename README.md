# Miniprojeto para processo seletivo do URBIS

Este jogo foi feito com o propósito de educar sobre o planejamento de um projeto de sustentabilidade de uma cidade. Nele, o jogador toma o controle de uma Secretaria de Meio Ambiente e Urbanismo de uma pequena cidade e deve gerenciar recursos para tornar a cidade sustentável, afim de reduzir o impacto no meio ambiente que a cidade causaria em futuras gerações.

Feito por Gabriel Paes Landim de Lucena.

Neste documento, será reunido as especificações do design deste projeto, como pré-planejamento, elaboração de game design, arquitetura/estrutura de projeto e especificações do código.

# Como jogar

Existem 4 construções no jogo - Casas, hospitais, delegacias e paradas de ônibus.

Casas precisam de delegacias e hospitais em seu alcance, se não elas tem um índice felicidade reduzido. Com um índice de felicidade reduzido, elas pagam menos impostos. 

Hospitais e delegacias aumentam a felicidade das casas. Caso uma casa já tenha uma dessas construções em seu alcance, a felicidade aumentada é menor.

Paradas de ônibus reduzem a emissão de gás carbono nas casas.

Para checar mais informações de uma construção (impostos por segundo, se uma casa tem acesso a delegacia, alcance do hospital etc) basta clicar na construção. Há um botão para pausar o tempo para poder analisar cada construção individualmente caso necessário.

Se um dos índices da cidade estiverem abaixo do mínimo, isto é, o texto do índice ficará vermelho, você terá 5 minutos para corrigir os índices. Caso não seja feito nada, o jogo acaba e o jogador perde.

Você pode a qualquer momento do jogo apertar o botão esc para salvar seu progresso e continuar de onde parou. **Entretanto, se você clicar no botão "Novo Jogo" seus dados salvos serão deletados**.

# Pré-planejamento

Antes do desenvolvimento deste projeto, procurei me informar sobre o desenvolvimento urbano sustentável. Para isso, entrei em contato com graduandos de cursos de Arquitetura e Urbanismo e pesquisei online sobre o tópico, onde que cheguei a [alguns vídeos do IBGE](https://www.youtube.com/watch?v=am2WOYu4iFc), os [Objetivos do Desenvolvimento Sustentável da ONU](https://brasil.un.org/index.php/pt-br/sdgs/11) e ao Pequeno Manual do Projeto Sustentável.

Feito isso, após pensar em muitas ideias, resolvi fazer um jogo de simulação de gerenciamento. Algumas das minhas referências são SimCity e principalmente, Cities: Skylines.

SimCity foi criado em 1989 e é responsável por criar o seu gênero, além de ser uma das séries de jogos mais renomadas do mundo. 

Cities: Skylines foi lançado em 2015 para PC e no seu primeiro mês, vendeu mais de 1 milhão de cópias e teve uma excelente recepção crítica. Outro ponto positivo de Cities: Skylines é que [ele foi usado para planejamento urbano pela cidade de Estocolmo](https://www.gamasutra.com/view/news/267926/Did_you_know_Stockholm_used_Cities_Skylines_for_urban_planning.php), mostrando o potencial do jogo. 

Fontes:

[https://pt.wikipedia.org/wiki/Cities:_Skylines](https://pt.wikipedia.org/wiki/Cities:_Skylines#Recep%C3%A7%C3%A3o)

[https://www.gamasutra.com/view/news/267926/Did_you_know_Stockholm_used_Cities_Skylines_for_urban_planning.php](https://www.gamasutra.com/view/news/267926/Did_you_know_Stockholm_used_Cities_Skylines_for_urban_planning.php)

# Game design

Resumindo o [objetivo das cidades e comunidades sustentáveis das Nações Unidas](https://brasil.un.org/pt-br/sdgs/11):

- Garantir a todos habitação segura, adequada e acessível;
- Proporcionar acesso a sistemas seguros e acessíveis de transporte;
- Reduzir o impacto ambiental negativo per capita das cidades, principalmente em relação a poluição do ar;
- Prover acesso a espaços públicos seguros, acessíveis e verdes;

Com isso em mente, vamos aos seguintes tópicos.

## Variáveis globais

O jogo será composto por várias variáveis, que irão persistir pelo decorrer do jogo. São elas:

- Emissão de poluição - Taxa de emissão de poluição na cidade. Afeta a taxa de impostos recebidos. Se o valor ficar muito alto, o jogador perde.
- Impostos - Quanto de dinheiro é recebido em cada segundo no jogo. Se o valor ficar muito baixo por muito tempo, o jogo acaba com o jogador perdendo.
- Fundo - Imposto acumulado que não está sendo usado. O valor pode acabar ficando negativo, o que faz com que não seja possível construir novas construções.
- Felicidade - Felicidade dos habitantes da cidade. Afeta os impostos recebidos. Se a cidade ficar infeliz por muito tempo, o jogador perde o jogo.

Com isso, teremos as seguintes estruturas no nosso jogo, que alterarão as variáveis acima.

- Casa - Estrutura base do jogo. Fonte de impostos, felicidade e gás carbono.
- Parada de ônibus - Todas as casas a um certo raio de distância da parada emitirão menos gás carbônico.
- Hospital - Todas as casas a um certo raio de distância do hospital produzirão mais felicidade.
- Delegacia - Todas as casas a um certo raio de distância da delegacia produzirão mais felicidade.

Para garantir o desafio de gerenciar a cidade, casas aparecerão aleatoriamente e o jogador deve garantir que elas sejam sustentáveis (tenham acessibilidade a hospitais e delegacias). As casas aparecerão adjacente umas as outras, para garantir que elas não ficarão extremamente espalhadas.

## Escopo

Devido a quantidade de tempo disponível para este projeto, escolhi fazer um protótipo que poderia ser facilmente expandido caso mais tempo de desenvolvimento fosse disponibilizado.

# Programação

Para esse projeto, foram usados os padrões de projeto singleton e observer.

O Singleton foi usado no CityManager, para armazenar variáveis como o tanto de dinheiro e coletar os impostos das casas, e no CityGUIManager para configurar valores da UI.

Já o observer foi usado bastante no código, pois evita classes acopladas e pode ser usado para reduzir o número e complexidade de Update()s rodando no jogo, aumentando a performance. O Observer foi usado majoritariamente para entrada do jogador.

## Scripts já existentes

Os seguintes scripts já tem parte da sua implementação criada anteriormente:

Assets\Scripts\UI/PauseMenuAuxiliar.cs

Assets\Scripts\DataManegement\Save system/SaveDataManager.cs

D:\Lucena\Documents\Repositories\urbis-submission-project\URBIS - Project\Assets\Scripts\Auxiliary/EventAuxiliar.cs

**Os demais assets foram feitos durante o desenvolvimento deste projeto.**

Caso necessário, eu posso provar que fui eu quem criou estes scripts.
