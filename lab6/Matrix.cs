using System;

namespace lab6
{
    public class Matrix
    {
        private double[,] elements;

        public int Rows => elements.GetLength(0);
        public int Columns => elements.GetLength(1);

        public Matrix(double[,] elements)
        {
            ValidateElements(elements);
            this.elements = elements;
        }

        public Matrix(int rows, int columns)
        {
            ValidateDimensionsSizes(rows, columns);
            elements = new double[rows, columns];
        }

        public double this[int row, int column]
        {
            get => GetElement(row, column);
            set => SetElement(row, column, value);
        }

        public double GetElement(int row, int column)
        {
            EnsureDimensionsAreInBounds(row, column);
            return elements[row, column];
        }

        public void SetElement(int row, int column, double value)
        {
            EnsureDimensionsAreInBounds(row, column);
            elements[row, column] = value;
        }

        public static Matrix operator *(Vector3d lhs, Matrix rhs)
        {
            return lhs.ToMatrix() * rhs;
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            EnsureMatricesAreMultipliable(lhs, rhs);
            var result = new Matrix(lhs.Rows, rhs.Columns);
            for (int i = 0; i < lhs.Rows; i += 1)
            {
                for (int j = 0; j < rhs.Columns; j += 1)
                {
                    double dotProduct = 0;
                    for (int r = 0; r < rhs.Rows; r += 1)
                    {
                        dotProduct += lhs[i, r] * rhs[r, j];
                    }
                    result[i, j] = dotProduct;
                }
            }
            return result;
        }

        private static void ValidateElements(double[,] elements)
        {
            ValidateDimensionsSizes(elements.GetLength(0), elements.GetLength(1));
        }

        private static void ValidateDimensionsSizes(int rows, int columns)
        {
            if (rows < 0 || columns < 0)
            {
                throw new ArgumentException("Aren't positive");
            }
        }

        private void EnsureDimensionsAreInBounds(int row, int column)
        {
            EnsureDimensionsAreInBounds(row, column, Rows, Columns);
        }

        private static void EnsureDimensionsAreInBounds(int row, int column, int rowsBound, int columnsBound)
        {
            if (row > rowsBound || column > columnsBound)
            {
                throw new IndexOutOfRangeException("Out of bounds");
            }
        }

        private static void EnsureMatricesAreMultipliable(Matrix lhs, Matrix rhs)
        {
            if (lhs.Columns != rhs.Rows)
            {
                throw new ArgumentException("Aren't multipliable");
            }
        }
    }
}
