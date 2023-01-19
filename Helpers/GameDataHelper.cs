namespace number_game.Helpers;

public class GameDataHelper
{
    public enum Op
    {
        Add = 1,
        Sub = 2
    }

    public struct Game
    { 
        public (int, int, int) Operands;
        public Op Op;
        public List<int> Options;
    }
    
    private int _limit;
    private int _numberOfDistractedAnswers;
    private static readonly int GeneralSeed = DateOnly.FromDateTime(DateTime.Now).DayNumber;
    
    public void Setup(int limit, int otherQuestions)
    {
        _limit = limit;
        _numberOfDistractedAnswers = otherQuestions;
    }
    
    private (int, int, int) OperandGenerator(Op op, int seed)
    {
        var rnd = new Random(GeneralSeed + (int) op + seed);
        var operand1 = rnd.Next(_limit);
        var operand2 = rnd.Next(_limit);
        if (op is Op.Sub && operand1 < operand2)
        {
            (operand1, operand2) = (operand2, operand1);
        }
        var answer = op switch {
            Op.Add => operand1 + operand2,
            Op.Sub => operand1 - operand2,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)};
        return (operand1, operand2, answer);
    }

    private List<int> AnswerOptions(int answer)
    {
        var rnd = new Random(GeneralSeed);
        List<int> distractedItems = new() { answer };
        for (int idx = 0; idx < _numberOfDistractedAnswers; idx++)
        {
            distractedItems.Add(answer + rnd.Next(_limit) - _limit / 2);
        }
        return Shuffle(distractedItems.Distinct().ToList());
    }

    private List<T> Shuffle<T>(List<T> input)
    {
        var numberOfChanges = (new Random()).Next(50);
        var length = input.Count();
        List<T> result = new(input);
        var rnd = new Random(Seed: numberOfChanges);
        for (int i = 0; i < numberOfChanges; i++)
        {
            var idx1 = Int32.Abs(rnd.Next(10 * length) % length);
            var idx2 = Int32.Abs(rnd.Next(10 * length) % length);
            (result[idx1], result[idx2]) = (result[idx2], result[idx1]);
        }
        return result;
    }

    public IEnumerable<Game>? MakeQuestion(int numberOfQuestions)
    {
        for (var i = 0; i < numberOfQuestions; i++)
        {
            var seed = (new Random()).Next(20);
            var operation = (new Random()).Next(2) > 0 ? Op.Add : Op.Sub;
            var operands = OperandGenerator(operation, seed);
            yield return new Game { Operands = operands, Op = operation, Options = AnswerOptions(operands.Item3) };   
        }
    }
}