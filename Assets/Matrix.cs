using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Matrix
{
    float[,] matrix;
    int colums;
    int rows;

    float[,] GetMatrix() => matrix;
    float GetMatrixElement(int x, int y) => matrix[x, y];
    public int Colums => colums;
    public int Rows => rows;

    System.Random random = new Random();
    public Matrix(int width, int height)
    {
        colums = (int)width;
        rows = (int)height;
        matrix = new float[colums, rows];
    }
    public Matrix(float[] array)
    {
        colums = 1;
        rows = array.Length;
        matrix = new float[colums, rows];
        for (int i = 0; i < colums; i++)
        {
            matrix[0, 1] = array[i];
        }
    }


    public void MultiplyWithScalar(float scalar)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                matrix[x, y] *= scalar;
            }
        }
    }


    public void ScalarAdd(float scalar)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                matrix[x, y] += scalar;
            }
        }
    }


    public void RandomizeValues(float min, float max)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                matrix[x, y] = Lerp(min, max, (float)random.NextDouble());
            }
        }
    }



    public void AddMatrix(Matrix m)
    {
        if (m.Colums == colums && m.Rows == rows)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < colums; x++)
                {
                    matrix[x, y] += m.GetMatrixElement(x, y);
                }
            }
        }
    }
    public void SubtractMatrix(Matrix m)
    {
        if (m.Colums == colums && m.Rows == rows)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < colums; x++)
                {
                    matrix[x, y] -= m.GetMatrixElement(x, y);
                }
            }
        }
    }
    public static Matrix SubtractToNewMatrix(Matrix a, Matrix b)
    {
        if (a.Colums == b.Colums && a.Rows == b.Colums)
        {
            Matrix output = new Matrix(a.Colums, a.Rows);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Colums; x++)
                {
                    output.SetValue(x, y, a.GetMatrixElement(x, y) - b.GetMatrixElement(x, y));
                }
            }
            return output;
        }
        return null;
    }


    /// <summary>
    /// Treats this object as the right matrix and the parameter as the left one
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public Matrix MultiplyWithMatrix(Matrix m)
    {
        if (this.rows == m.Colums)
        {
            Matrix result = new Matrix(this.Colums, m.Rows);

            for (int y = 0; y < result.rows; y++)
            {
                for (int x = 0; x < result.Colums; x++)
                {
                    float value = 0;
                    for (int k = 0; k < m.colums; k++)
                    {
                        float a = this.matrix[x, k];
                        value += a * m.GetMatrixElement(k, y);
                    }
                    result.SetValue(x, y, value);
                }
            }
            return result;
        }
        else
        {
            return null;
        }
    }
    public Matrix MultiplyWithMatrix2(Matrix m)
    {
        if (this.colums == m.Rows)
        {
            Matrix result = new Matrix(m.colums, this.rows);

            for (int y = 0; y < result.rows; y++)
            {
                for (int x = 0; x < result.Colums; x++)
                {
                    float value = 0;
                    for (int k = 0; k < m.colums; k++)
                    {
                        float a = this.matrix[x, k];
                        value += a * m.GetMatrixElement(k, y);
                    }
                    result.SetValue(x, y, value);
                }
            }
            return result;
        }
        else
        {
            return null;
        }
    }


    public Matrix GetTransposedCopy()
    {
        Matrix result = new Matrix(rows, colums);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                result.SetValue(y, x, this.matrix[x, y]);
            }
        }
        return result;
    }

    public void Map(Func<float, float, float> function)
    {
        // Apply a function to every element of matrix
        for (int y = 0; y < this.rows; y++)
        {
            for (int x = 0; x < this.colums; x++)
            {
                float val = matrix[x, y];
                matrix[x, y] = function(x, y);
            }
        }
    }
    public static Matrix MapCopy(Matrix target, Func<float, float, float> function)
    {
        Matrix copy = target.Clone();
        // Apply a function to every element of matrix
        copy.Map(function);
        return copy;
    }

    public float[] ToArray()
    {
        float[] array = new float[colums * rows];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                array[x + y * colums] = matrix[x, y];
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
                output += "|" + matrix[x, y].ToString();
            }
            output += "| \n";
        }
        return output;
    }
    public Matrix Clone() => this.MemberwiseClone() as Matrix;
    public void SetValue(int x, int y, float val)
    {
        matrix[x, y] = val;
    }
    float Lerp(float a, float b, float w)
    {
        return a + w * (b - a);
    }
}


