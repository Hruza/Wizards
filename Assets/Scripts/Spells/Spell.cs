public class Spell {
    public Spell(ElementDefinition element,Attack attack,SpellModifier modifier,SpellManager manager){
        this.manager = manager;
        this.element = element;
        this.modifier = modifier;
        this.attack = attack;
        attack.Initialise(this);
    }

    public ElementDefinition element{get;private set;}
    public SpellModifier modifier{get;private set;}
    public Attack attack{get;private set;}
    public SpellManager manager{get;private set;}
}