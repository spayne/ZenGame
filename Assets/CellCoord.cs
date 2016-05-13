using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct CellCoord : IEquatable<CellCoord>
{
    public int row;
    public int col;

    public CellCoord(int row_in, int col_in)
    { row = row_in; col = col_in; }

    public bool Equals(CellCoord other)
    {
        return (row == other.row && col == other.col);
    }

    public override bool Equals(Object obj)
    {
        if (obj == null || !(obj is CellCoord))
            return false;

        return Equals((CellCoord)obj);
    }
    public override int GetHashCode()
    {
        return row.GetHashCode() ^ col.GetHashCode();
    }
    public static bool operator ==(CellCoord x, CellCoord y)
    {
        return x.row == y.row && x.col == y.col;
    }
    public static bool operator !=(CellCoord x, CellCoord y)
    {
        return !(x == y);
    }

}
/*
class SCellCoord : System.Object
{
    public int row;
    public int col;

    public CellCoord(int row_in, int col_in)
    { row = row_in; col = col_in; }


    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        CellCoord p = obj as CellCoord;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (row == p.col) && (col == p.row);
    }

    public bool Equals(CellCoord p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (col == p.col) && (row == p.row);
    }

    public override int GetHashCode()
    {
        return (row << 16) | col;
    }



    public static bool operator ==(CellCoord a, CellCoord b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return (a.row == b.row) && (a.col == b.col);
    }

    public static bool operator !=(CellCoord a, CellCoord b)
    {
        return !(a == b);
    }

}
*/