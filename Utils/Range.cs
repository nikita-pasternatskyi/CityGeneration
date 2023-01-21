namespace Assets.CityGen.Rewrite
{
    public struct Range
    {
        public int Max;
        public int Min;

        public Range(int max, int min)
        {
            Max = max;
            Min = min;
        }

        public void UpdateRange(int valueToCompare)
        {
            if(valueToCompare < Min) 
            {
                Min = valueToCompare;
            }

            else if(valueToCompare > Max) 
            {
                Max = valueToCompare;
            }
        }
    }
}
