using System.Collections.Generic;
using UnityEngine;

public static class CharacterDefaults
{
    public static readonly string AllCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,:;?!'\"-()@&#%+=*/";

    public static void EnsureCharacters(ProjectSettings ps)
    {
        if (ps.characters == null)
            ps.characters = new List<CharacterData>();

        if (ps.characters.Count > 0)
            return;

        foreach (char c in AllCharacters)
            ps.characters.Add(new CharacterData(c.ToString()));
    }
}

