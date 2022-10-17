using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    EXPRESSION 
    NOTE: might be implementing this wrong and dumb as f, might not even need the interface in this situation tbh
*/
public interface IExpression
{
    public abstract int Perform(int powerLevel);
}
public class Expression : IExpression
{
    protected int value;
    public Expression(int value)
    {
        this.value = value;
    }
    public Expression() { }
    public virtual int Perform(int powerLevel) { return 0; }
    public override string ToString()
    {
        return "Empty expression";
    }
    public void ChangeValue(int value)
    {
        this.value = value; 
    }
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
public class PlusExpression : Expression
{
    public PlusExpression(int value) : base(value) { }
    public PlusExpression() { }
    public override int Perform(int powerLevel)
    {
        return powerLevel + value;
    }
    public override string ToString()
    {
        return $"X+{value}";
    }
}
public class MultiplyExpression : Expression
{
    public MultiplyExpression(int value) : base(value) { }
    public MultiplyExpression() { }


    public override int Perform(int powerLevel)
    {
        return powerLevel * value;
    }
    public override string ToString()
    {
        return $"X*{value}";
    }
}
public class SubtractExpression : Expression
{
    public SubtractExpression(int value) : base(value) { }
    public SubtractExpression() { }

    public override int Perform(int powerLevel)
    {
        return powerLevel - value <= 0 ? 1 : powerLevel - value;
    }
    public override string ToString()
    {
        return $"X-{value}";
    }
}
public class DivideExpression : Expression
{
    public DivideExpression(int value) : base(value) { }
    public DivideExpression() { }
    public override int Perform(int powerLevel)
    {
        return powerLevel / value <= 1 ? 1 : powerLevel / value;
    }
    public override string ToString()
    {
        return $"X/{value}";
    }
}
public class EqualExpression : Expression
{
    public EqualExpression(int value) : base(value) { }
    public EqualExpression() { }
    public override int Perform(int powerLevel)
    {
        return value;
    }
    public override string ToString()
    {
        return $"X={value}";
    }
}
public class SquareRootExpression : Expression
{
    public SquareRootExpression(int value) : base(value) { }
    public SquareRootExpression() { }

    public override int Perform(int powerLevel)
    {
        int result = Mathf.RoundToInt(Mathf.Sqrt(powerLevel));
        return result <= 0 ? 1 : result;
    }
    public override string ToString()
    {
        return "√X";
    }
}
