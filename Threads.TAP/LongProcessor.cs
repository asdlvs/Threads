namespace Threads.TAP
{
    using System.Threading.Tasks;

    public class LongProcessor
    {
        public long Calculate(int initialValue)
        {
            var result = 0;
            for (int i = initialValue; i < int.MaxValue; i++)
            {
                result = result + i;
            }
            return result;
        }

        public Task<long> CalculateAsync(int initialValue)
        {
            return new Task<long>(() => Calculate(10));
        }
    }
}
