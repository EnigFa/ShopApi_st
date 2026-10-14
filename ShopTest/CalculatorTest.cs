using MyCalculator;

namespace ShopTest
{
    public class CalculatorTest
    {
        [Fact]
        public void SumTest()
        {
            // Arrange
            int a = 10, b = 20;
            Calculator calculator = new Calculator();

            // Act
            int result = calculator.Sum(a, b);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
        public void MultTest()
        {
            // Arrange
            int a = 10, b = 20;
            Calculator calculator = new Calculator();

            // Act
            int result = calculator.mult(a, b);

            // Assert
            Assert.Equal(200, result);
        }

        [Fact]
        public void MinTest()
        {
            // Arrange
            int a = 10, b = 20;
            Calculator calculator = new Calculator();

            // Act
            int result = calculator.Min(a, b);

            // Assert
            Assert.Equal(10, result);
        }
    }
}
