# Backfacing RPG - Estrutura Completa

## 📁 Estrutura Final de Arquivos

```
BackfacingRPG/
├── Core/
│   ├── Game1.cs                  # Classe principal do jogo
│   ├── GameStateManager.cs       # Gerenciador de estados
│   └── InputManager.cs           # Gerenciamento de input
├── Entities/
│   ├── Entity.cs                 # Classe base para entidades
│   ├── Player.cs                 # Classe do jogador
│   └── Enemy.cs                  # Classe dos inimigos
├── GameStates/
│   ├── BaseGameState.cs          # Estado base
│   ├── ExplorationState.cs       # Estado de exploração
│   ├── CombatState.cs           # Estado de combate
│   └── DialogueState.cs         # Estado de diálogo
├── Combat/
│   ├── Enums.cs                 # Enums do sistema de combate
│   ├── CombatAction.cs          # Ações de combate
│   └── CombatManager.cs         # Gerenciador de combate
├── UI/
│   ├── DialogueBox.cs           # Caixa de diálogo
│   ├── HealthBar.cs             # Barras de vida
│   └── UIHelpers.cs             # Helpers para UI
├── Utils/
│   ├── ColorPalette.cs          # Paleta de cores
│   └── DrawingHelpers.cs        # Helpers de desenho
├── Content/
│   ├── Content.mgcb             # Pipeline de conteúdo
│   ├── Fonts/
│   │   ├── DialogFont.spritefont
│   │   └── UIFont.spritefont
│   └── Textures/                # (Para sprites futuros)
├── Program.cs                   # Ponto de entrada
├── BackfacingRPG.csproj        # Arquivo de projeto
└── README.md                   # Este arquivo
```

## 🎯 Funcionalidades Implementadas

### ✅ Sistema de Estados
- **ExplorationState**: Movimentação livre, interação com o ambiente
- **CombatState**: Sistema de combate por turnos completo
- **DialogueState**: Sistema de diálogos com múltiplas opções

### ✅ Sistema de Entidades
- **Player**: Personagem com stats, experiência, ouro, movimento
- **Enemy**: Inimigos com IA, diferentes tipos e comportamentos
- **Entity**: Classe base com sistema de vida, stats e desenho

### ✅ Sistema de Combate
- Combate por turnos
- IA dos inimigos (Agressivo, Defensivo, Inteligente, Covarde, Berserker)
- Sistema de ações (Atacar, Defender, Fugir, Item)
- Cálculo de dano e defesa
- Sistema de recompensas (XP e ouro)

### ✅ Sistema de UI
- Caixas de diálogo animadas
- Barras de vida com transições suaves
- Sistema de menus navegáveis
- Efeitos visuais (pulsação, seleção, etc.)

### ✅ Sistema Visual (Placeholders)
- **Jogador**: Retângulo azul com detalhes (mostrando de costas)
- **Inimigos**: Cores diferentes por tipo com detalhes específicos
- **UI**: Interface completa com bordas e animações
- **Ambiente**: Elementos básicos de cenário

## 🚀 Como Começar

### 1. Criando o Projeto
```bash
# Criar pasta do projeto
mkdir BackfacingRPG
cd BackfacingRPG

# Copiar todos os arquivos .cs para as pastas corretas
# Copiar BackfacingRPG.csproj para a raiz
# Copiar Program.cs para a raiz
```

### 2. Estrutura de Pastas
Crie exatamente esta estrutura:
```
BackfacingRPG/
├── Core/
├── Entities/
├── GameStates/
├── Combat/
├── UI/
├── Utils/
└── Content/
```

### 3. Compilação e Execução
```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Executar
dotnet run
```

## 🎮 Controles

### Exploração
- **WASD / Setas**: Movimentação
- **E**: Interagir com ambiente/inimigos
- **T**: Iniciar combate de teste
- **I**: Mostrar informações do jogador
- **F1**: Toggle debug info

### Combate
- **Setas ↑↓**: Navegar ações/alvos
- **Enter**: Confirmar seleção
- **Esc**: Cancelar/Voltar

### Diálogos
- **Setas**: Navegar opções
- **Enter**: Confirmar
- **Esc**: Cancelar

## 🎨 Sistema de Cores e Visual

### Cores dos Placeholders
- **Jogador**: Azul (`#4080FF`) - Retângulo 24x32
- **Goblin**: Verde (`#78A050`) - Retângulo 28x28
- **Orc**: Marrom (`#644C3C`) - Retângulo 28x28
- **Esqueleto**: Branco-osso (`#DCDCC8`) - Retângulo 28x28
- **Dragão**: Roxo (`#A028C8`) - Retângulo 28x28
- **Slime**: Verde-lima (`#50C878`) - Retângulo 28x28
- **Lobo**: Cinza (`#646464`) - Retângulo 28x28

### Detalhes Visuais Implementados
- **Jogador**: Cabeça mais escura, "mochila" nas costas
- **Goblin**: "Orelhas" pontiagudas
- **Orc**: "Presas" brancas
- **Esqueleto**: Linhas de "costelas"
- **Dragão**: "Asas" dos lados
- **Slime**: Formato mais arredondado
- **Lobo**: "Cauda" atrás

## 🔧 Adicionando Sprites Reais

Quando você tiver sprites, substitua o método `DrawPlaceholder` por `DrawSprite`:

```csharp
// Em Entity.cs - descomente e use:
protected virtual void DrawSprite(SpriteBatch spriteBatch, Texture2D spriteTexture)
{
    var drawPosition = Position;
    var sourceRect = new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height);
    
    spriteBatch.Draw(
        spriteTexture,
        drawPosition,
        sourceRect,
        TintColor,
        0f,
        Vector2.Zero,
        Scale,
        SpriteEffects.None,
        0f
    );
}

// Em Game1.cs - descomente:
_playerTexture = Content.Load<Texture2D>("Textures/player");
_enemyTexture = Content.Load<Texture2D>("Textures/enemy");
```

## 📈 Próximas Funcionalidades

### Prioridade Alta
1. **Sistema de Inventário** - Items, equipamentos, consumíveis
2. **Mapas/Cenários** - Múltiplos ambientes, transições
3. **Save/Load** - Sistema de salvamento
4. **Sons** - Efeitos sonoros e música

### Prioridade Média
1. **Magias/Habilidades** - Sistema de magic/skills
2. **NPCs** - Personagens não-jogáveis
3. **Quests** - Sistema de missões
4. **Loja** - Sistema de compra/venda

### Prioridade Baixa
1. **Animações** - Sprites animados
2. **Partículas** - Efeitos visuais
3. **Multiplayer** - Funcionalidades online

## 🔍 Arquitetura do Código

### Padrões Utilizados
- **State Pattern**: GameStateManager para diferentes estados do jogo
- **Entity Pattern**: Sistema base para Player/Enemy
- **Strategy Pattern**: IA dos inimigos com diferentes comportamentos
- **Observer Pattern**: Eventos no sistema de combate
- **Factory Pattern**: Criação de inimigos e diálogos

### Extensibilidade
- **Novos Estados**: Herdar de `BaseGameState`
- **Novos Inimigos**: Usar factory methods em `Enemy.cs`
- **Novas Ações**: Adicionar em `CombatActionType` enum
- **Nova UI**: Usar helpers em `UIHelpers.cs`

## 🐛 Debug e Desenvolvimento

### Informações de Debug (F1)
- Estado atual do jogo
- Posição do jogador
- Número de inimigos vivos
- Inimigo mais próximo
- Fase do combate

### Logs Úteis
- Ações de combate
- Mudanças de estado
- Interações do jogador
- Cálculos de dano

## 📝 Notas Importantes

1. **Sem LocalStorage**: O código não usa APIs de navegador
2. **Placeholders**: Todos os gráficos são retângulos coloridos
3. **Fontes**: Sistema funciona sem fontes (usa placeholders)
4. **Extensível**: Arquitetura preparada para expansão
5. **MonoGame**: Compatible com .NET 8.0

## 🎉 Resultado Final

Este projeto te dá uma base sólida de RPG com:
- ✅ Personagem funcionando (placeholder visual)
- ✅ Sistema de combate completo
- ✅ IA de inimigos variada
- ✅ Interface responsiva
- ✅ Estados bem organizados
- ✅ Código modular e extensível
- ✅ Controles intuitivos
- ✅ Sistema de progressão (XP/Level/Gold)

**Agora você pode focar em adicionar conteúdo, sprites e mecânicas específicas do seu jogo!**