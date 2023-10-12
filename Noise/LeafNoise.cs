/*************************************************
 *  Author: Ian
 *
 *  Project: General Utility
 *
 *  Date: 8/5/22  
 *  
 *  Various Noise and Random methods based on Squirrel3 Noise
 *************************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LeafNoise
{
    private static uint seed = (uint)System.DateTime.Now.Millisecond;
    

    //A series of random methods each randomizes the static seed when used

    ///<returns>Random roll against P(probability).</returns>
    public static bool RandomChance(float probability)
    {
        //Edge case if chance is 0 & 0 is rolled should return false
        if (probability == 0) return false;

        return (float)Random()/uint.MaxValue <= probability;
    }
    #region Random Number
    /// <summary>Returns a random uint.</summary>
    public static uint Random()
    {
        seed = Noise((int)seed, 0);
        return seed;
    }
    /// <summary>Returns a random float [min, max]. (Inclusive)</summary>
    public static float Random(float min, float max) => NoiseToRange(Random(), new Vector2(min,max));
    /// <summary>Returns a random float [range.x, range.y]. (Inclusive)</summary>
    public static float Random(Vector2 range) => Random(range.x, range.y);
    /// <summary>Returns a random integer [min, max]. (Inclusive)</summary>
    public static int Random(int min, int max) => NoiseToRange(Random(), new Vector2Int(min, max));
    /// <summary>Returns a random integer [range.x, range.y]. (Inclusive)</summary>
    public static int Random(Vector2Int range) => Random(range.x, range.y);

    #endregion
    #region Random Element
    /// <returns>Random item from items.</returns>
    public static T Random<T>(List<T> items) => items[Random(0, items.Count - 1)];
    ///<summary>If no weights are passed in assumes equal weights. Otherweise lengths of Lists must be equal.</summary>
    /// <returns>Random item from item list based on weights.</returns>
    public static T RandomWeighted<T>(List<T> items, List<float> weights)
    {
        if (weights.Count == 0) return Random(items);

        //Gets Total Weight
        float totalWeight = 0;
        foreach (float w in weights)
            totalWeight += w;

        //Get rand
        float rand = Random(0f, totalWeight);
        float weightIndex = 0;
        for(int i = 0; i < items.Count; i++)
        {
            weightIndex += weights[i];
            if (weightIndex > rand)
                return items[i];
        }

        throw new System.Exception($"LeafNoise: rand weight larger than totalweight! {items[0]}");
    }

    /// <summary>Returns a random item from the list of WeightedElements based on each Weighted's weight.</summary>
    public static T RandomWeighted<T>(List<Weighted<T>> weightedElements)
    {
        List<float> weights = new();
        List<T> elements = new();

        foreach (Weighted<T> weightedElement in weightedElements)
        {
            weights.Add(weightedElement.weight);
            elements.Add(weightedElement.element);
        }

        return RandomWeighted(elements, weights);
    }
    /// <summary>Returns a random item from the array of WeightedElements based on each Weighted's weight.</summary>
    public static T RandomWeighted<T>(Weighted<T>[] weightedElements)
    {
        List<float> weights = new();
        List<T> elements = new();

        foreach (Weighted<T> weightedElement in weightedElements)
        {
            weights.Add(weightedElement.weight);
            elements.Add(weightedElement.element);
        }

        return RandomWeighted(elements, weights);
    }
    /// <summary>Returns a random item from the List of IWeighted based on each IWeighted's weight.</summary>
    public static IWeighted RandomWeighted(List<IWeighted> weightedElements)
    {
        List<float> weights = new();

        foreach (IWeighted weightedElement in weightedElements)
        {
            weights.Add(weightedElement.GetWeight());
        }

        return RandomWeighted(weightedElements, weights);
    }
    /// <summary>Returns a random item from the Array of IWeighted based on each IWeighted's weight.</summary>
    public static IWeighted RandomWeighted(IWeighted[] weightedElements)
    {
        List<float> weights = new();
        List<IWeighted> weightedElementList = new();

        foreach (IWeighted weightedElement in weightedElements)
        {
            weightedElementList.Add(weightedElement);
            weights.Add(weightedElement.GetWeight());
        }

        return RandomWeighted(weightedElementList, weights);
    }

    ///<summary>
    ///Interface for passing in a list of weighted classes.
    ///Weighted<Object> can be used when object is a scriptable object.
    ///</summary>
    public interface IWeighted 
    { 
        public float GetWeight(); 
    }
    #endregion
    #region Noise
    /// <summary>Squirel3 1D Noise.</summary>
    public static uint Noise(int position, uint seed)
    {
        uint bigP1 = 0x68E31DA4;
        uint bigP2 = 0xB5297A4D;
        uint bigP3 = 0x1B56C4E9;

        uint mangled = (uint)position;
        mangled *= bigP1;
        mangled += seed;
        mangled ^= (mangled >> 8);
        mangled += bigP2;
        mangled ^= (mangled << 8);
        mangled *= bigP3;
        mangled ^= (mangled >> 8);
        return mangled;
    }
    /// <summary>Squirel3 2D Noise.</summary>
    public static uint Noise(int x, int y, uint seed) => Noise(x + y * 198491317, seed);
    /// <summary>Returns a linear interpolation between 1D noise values.</summary>
    public static uint Noise(float position, uint seed)
    {
        uint floorNoise = Noise(Mathf.FloorToInt(position), seed);
        uint ceilNoise = Noise(Mathf.CeilToInt(position), seed);


        return Lerp(floorNoise, ceilNoise, position % 1);
    }
    public static float Noise(float position, uint seed, Vector2 range) => NoiseToRange(Noise(position, seed), range);
    /// <summary>Returns a linear interpolation between 2D noise values.</summary>
    public static uint Noise(float x, float y, uint seed)
    {
        //Get Interpolations between x with floored y and x with ceiling y
        //uint x1toX2YFloorLerped = Noise(Noise(x + Mathf.Floor(y * 198491317), seed) + x % 1, seed);
        //uint x1toX2YCeilLerped  = Noise(Noise(x + Mathf.Ceil(y * 198491317), seed) + x % 1, seed);

        uint floorNoise = Noise(Mathf.FloorToInt(x), Mathf.FloorToInt(y), seed);
        uint ceilNoise =  Noise(Mathf.CeilToInt(x), Mathf.FloorToInt(y), seed);
        uint x1toX2YFloorLerped = Lerp(floorNoise, ceilNoise, x % 1);


        floorNoise =      Noise(Mathf.FloorToInt(x), Mathf.CeilToInt(y), seed);
        ceilNoise =       Noise(Mathf.CeilToInt(x), Mathf.CeilToInt(y), seed);
        uint x1toX2YCeilLerped = Lerp(floorNoise, ceilNoise, x % 1);

        //Return Interpolation between those two values
        return Lerp(x1toX2YFloorLerped, x1toX2YCeilLerped, y % 1);
    }
    public static float Noise(float x, float y, uint seed, Vector2 range) => NoiseToRange(Noise(x, y, seed), range);
    #endregion
    #region Util
    /// <summary>Converts noise into a value [range.x, range.y]. (Inclusive)</summary>
    public static float NoiseToRange(uint noise, Vector2 range) => range.x + (float)noise / uint.MaxValue * (range.y - range.x);
    /// <summary>Converts noise into a value [range.x, range.y]. (Inclusive)</summary>
    public static int NoiseToRange(uint noise, Vector2Int range)
    {
        if (noise == uint.MaxValue) return range.y;
        else return Mathf.FloorToInt(range.x + (float)noise / uint.MaxValue * (range.y + 1 - range.x));
    }

    /// <summary> uint Lerp </summary>
    public static uint Lerp(uint from, uint to, float lerpPercent) => (uint)( (1-lerpPercent) * from   +   lerpPercent * to);
    #endregion
}
