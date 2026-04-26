// MIT License
// 
// Copyright (c) 2021 Fletcher Cole
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;

namespace FletcherLibraries
{
    [System.Serializable]
    public struct IntegerVector
    {
        public int X;
        public int Y;

        public IntegerVector(int x = 0, int y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public IntegerVector(Vector2 v)
        {
            this.X = Mathf.RoundToInt(v.x);
            this.Y = Mathf.RoundToInt(v.y);
        }

        public static IntegerVector Zero { get { return new IntegerVector(); } }

        public static IntegerVector operator +(IntegerVector v1, IntegerVector v2)
        {
            return new IntegerVector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static IntegerVector operator -(IntegerVector v1, IntegerVector v2)
        {
            return new IntegerVector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static IntegerVector operator *(IntegerVector v, int i)
        {
            return new IntegerVector(v.X * i, v.Y * i);
        }

        public static IntegerVector operator /(IntegerVector v, int i)
        {
            return new IntegerVector(v.X / i, v.Y / i);
        }

        public static implicit operator Vector2(IntegerVector v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static implicit operator IntegerVector(Vector2 v)
        {
            return new IntegerVector(v);
        }

        public static bool operator ==(IntegerVector v1, IntegerVector v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(IntegerVector v1, IntegerVector v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntegerVector)
                return this == (IntegerVector)obj;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
