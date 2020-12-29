using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Matrix
{
    float[][] matrix;
    int colums;
    int rows;





    Random random = new Random();
    public float[][] GetMatrix() => matrix;
    public float GetMatrixElement(int row, int collum) => matrix[row][collum];
    public int Colums => colums;
    public int Rows => rows;

    public Matrix(int rowCount, int collumCount)
    {
        colums = collumCount;
        rows = rowCount;
        matrix = new float[rowCount][];
        for (int i = 0; i < rowCount; i++)
        {
            matrix[i] = new float[collumCount];
        }
    }
    public Matrix(float[] floatArray)
    {
        rows = floatArray.Length;
        colums = 1;
        matrix = new float[rows][];
        for (int i = 0; i < rows; i++)
        {
            matrix[i] = new float[] { floatArray[i] };
        }
    }
    public Matrix(FlatMatrix flatMatrix)
    {
        rows = flatMatrix.rows;
        colums = flatMatrix.colums;
        matrix = new float[rows][];
        for (int y = 0; y < rows; y++)
        {
            matrix[y] = new float[colums];
            for (int x = 0; x < colums; x++)
            {
                matrix[y][x] = flatMatrix.matrix[x + y * colums];
            }
        }
    }


    public void RandomizeValues()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                matrix[y][x] = (float)random.NextDouble() * 2f - 1f;
            }
        }
    }


    public void Add(Matrix other)
    {
        if (this.rows != other.Rows || this.Colums != other.Colums)
        {
            throw new Exception("Both Matrixes needs to be the same size");
            //return;
        }
        this.Map((int i, int j, float val) =>
        {
            return val + other.matrix[i][j];
        });
    }

    public void Add(float[] values)
    {
        if (this.rows != values.Length || this.Colums != 1)
        {
            throw new Exception("Both Matrixes needs to be the same size");
            //return;
        }
        this.Map((int i, int j, float val) =>
        {
            return val + values[i];
        });
    }
    public static void Add(ref float[] target, float[] other)
    {
        if (target.Length != other.Length)
        {
            throw new Exception("Both arrays needs to be the same size");
        }
        for (int i = 0; i < target.Length; i++)
        {
            target[i] += other[i];
        }
    }

    public static Matrix Subtract(Matrix a, Matrix b)
    {
        if (a.rows != b.Rows || a.Colums != b.Colums)
        {
            throw new Exception("Both Matrixes needs to be the same size");
            //return;
        }
        Matrix outMatrix = new Matrix(a.Rows, b.Colums);
        outMatrix.Map((int i, int j) =>
        {
            return a.GetMatrixElement(i, j) - b.GetMatrixElement(i, j);
        });
        return outMatrix;
    }




    public void MultiplyScalarProduct(float scalar)
    {
        this.Map((float e) => e * scalar);
    }




    public void MultiplyHadamardProduct(Matrix other)
    {
        if (this.rows != other.Rows || this.Colums != other.Colums)
        {
            throw new Exception("Both Matrixes needs to be the same size");
            //return;
        }
        // hadamard product
        this.Map((int i, int j, float e) => e * other.GetMatrixElement(i, j));
    }




    public static Matrix MatrixMultiplication(Matrix a, Matrix b)
    {
        if (a.Colums != b.Rows)
        {
            throw new Exception("Matrix A width needs to be same as matrix B height");
            //return null;
        }
        Matrix newMatrix = new Matrix(a.Rows, b.Colums);
        newMatrix.Map((int i, int j) =>
        {
            float sum = 0;
            for (int k = 0; k < a.Colums; k++)
            {
                float av = a.GetMatrixElement(i, k);
                float bv = b.GetMatrixElement(k, j);
                sum += av * bv;
            }
            return sum;
        });
        return newMatrix;
    }

    public static float[] MatrixMultiplicationColumMatch(Matrix a, float[] b)
    {
        if (a.Colums != b.Length)
        {
            throw new Exception("Matrix A width needs to be same as matrix B height");
            //return null;
        }
        float[] newMatrix = new float[a.rows];
        for (int y = 0; y < a.rows; y++)
        {
            float sum = 0;
            for (int x = 0; x < a.colums; x++)
            {
                float av = a.GetMatrixElement(y, x);
                float bv = b[x];
                sum += av * bv;
            }
            newMatrix[y] = sum;
        }
        return newMatrix;
    }

    public static float[] MatrixMultiplicationRowMatch(Matrix a, float[] b)
    {
        //if (a.rows != b.Length)
        //{
        //    throw new Exception("Matrix A width needs to be same as matrix B height");
        //    //return null;
        //}

        //[,]
        //[,]
        //[,]
        //[,]
        //[,,,]

        float[] newMatrix = new float[a.colums];
        for (int y = 0; y < a.colums; y++)
        {
            float sum = 0;
            for (int x = 0; x < a.rows; x++)
            {
                float av = a.GetMatrixElement(y, x);
                float bv = b[x];
                sum += av * bv;
            }
            newMatrix[y] = sum;
        }
        return newMatrix;
    }
    public static Matrix MatrixMultiplication(float[] a, Matrix b)
    {
        if (b.rows != 1)
        {
            throw new Exception("Matrix A width needs to be same as matrix B height");
        }
        Matrix newMatrix = new Matrix(a.Length, b.Colums);
        newMatrix.Map((int i, int j) =>
        {
            float sum = 0;
            for (int k = 0; k < a.Length; k++)
            {
                float av = a[k];
                float bv = b.GetMatrixElement(k, j);
                sum += av * bv;
            }
            return sum;
        });
        return newMatrix;
    }




    public static Matrix MultiplyDotProduct(Matrix a, float[] b)
    {
        if (a.Colums != b.Length)
        {
            throw new Exception("Matrix A width needs to be same as matrix B height");
            //return null;
        }
        Matrix newMatrix = new Matrix(a.Rows, 1);
        newMatrix.Map((int i, int j) =>
        {
            float sum = 0;
            for (int k = 0; k < a.Colums; k++)
            {
                float av = a.GetMatrixElement(i, k);
                float bv = b[k];
                sum += av * bv;
            }
            return sum;
        });
        return newMatrix;
    }




    public void Map(System.Func<float, float> function)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < colums; j++)
            {
                float value = matrix[i][j];
                matrix[i][j] = function(value);
            }
        }
    }
    public void Map(System.Func<int, int, float> function)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < colums; j++)
            {
                matrix[i][j] = function(i, j);
            }
        }
    }
    public void Map(System.Func<int, int, float, float> function)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < colums; j++)
            {
                float value = matrix[i][j];
                matrix[i][j] = function(i, j, value);
            }
        }
    }





    public static Matrix MapClone(Matrix mat, Func<float, float> function)
    {
        Matrix outMatrix = new Matrix(mat.Rows, mat.Colums);
        for (int i = 0; i < outMatrix.Rows; i++)
        {
            for (int j = 0; j < outMatrix.Colums; j++)
            {
                outMatrix.SetValue(i, j, function(mat.GetMatrixElement(i, j)));
            }
        }
        outMatrix.Map(function);
        return outMatrix;
    }

    
    public Matrix GetTransposedClone()
    {
        Matrix outMatrix = new Matrix(colums, rows);
        outMatrix.Map((int i, int j) => matrix[j][i]);
        return outMatrix;
    }



    public float[] ToArray()
    {
        float[] array = new float[colums * rows];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                array[x + y * colums] = matrix[y][x];
            }
        }
        return array;
    }



    public string ToString()
    {
        string output = "";
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                output += "|" + matrix[y][x].ToString();
            }
            output += "| \n";
        }
        return output;
    }
    public Matrix Clone() => this.MemberwiseClone() as Matrix;
    public void SetValue(int row, int colum, float val) => matrix[row][colum] = val;
    float Lerp(float a, float b, float w) => a + w * (b - a);
}


