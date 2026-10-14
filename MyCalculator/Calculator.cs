namespace MyCalculator
{
    public class Calculator
    {
        public int Sum(int a, int b)
        {
            return a + b;
        }

        public int Min(int a, int b)
        {
            if (a > b)
                return b;

            return a;
        }

        public int mult(int a, int b)
        {
            return a * b;
        }

        public int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Zero error");

            return a / b;
        }
    }
}
