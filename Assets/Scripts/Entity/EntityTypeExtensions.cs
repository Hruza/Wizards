public static class EntityTypeExtensions
{  
    public static bool HasFlag(this EntityType self, EntityType flag){
        return (self & flag) == flag;
    }

}

