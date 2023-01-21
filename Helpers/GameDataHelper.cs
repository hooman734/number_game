using System.Data.SqlTypes;

namespace number_game.Helpers;

public class GameDataHelper
{
    public enum Op
    {
        Add = 1,
        Sub = 2,
        Greater = 3,
        Smaller = 4,
        Times = 5,
        Divide = 6,
    }

    public struct Game
    { 
        public (int, int) Operands;
        public string Answer;
        public Op Op;
        public List<string>? Options;
    }
    
    private int _limit;
    private int _numberOfDistractedAnswers;
    private static readonly int GeneralSeed = DateOnly.FromDateTime(DateTime.Now).DayNumber;
    
    public void Setup(int limit, int otherQuestions)
    {
        _limit = limit;
        _numberOfDistractedAnswers = otherQuestions;
    }
    
    private (int, int, string) OperandGenerator(Op op, int seed)
    {
        var rnd = new Random(GeneralSeed + (int) op + seed);
        var operand1 = rnd.Next(_limit);
        var operand2 = rnd.Next(_limit);
        if (op is Op.Sub && operand1 < operand2)
        {
            (operand1, operand2) = (operand2, operand1);
        }
        var answer = op switch {
            Op.Add => (operand1 + operand2).ToString(),
            Op.Sub => (operand1 - operand2).ToString(),
            Op.Greater => (operand1 > operand2).ToString(),
            Op.Smaller => (operand1 < operand2).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)};
        return (operand1, operand2, answer);
    }

    private List<string> AnswerOptions<T>(T answer)
    {
        var rnd = new Random(GeneralSeed);
        List<string> distractedItems = new();

        try
        {
            var intAnswer = Convert.ToInt32(answer);
            distractedItems.Add(intAnswer.ToString());
            for (var idx = 0; idx < _numberOfDistractedAnswers; idx++)
            {
                distractedItems.Add((rnd.Next(_limit) + intAnswer - _limit / 2).ToString());
            }

            return Shuffle(distractedItems.Distinct().ToList());
        }
        catch
        {
            distractedItems!.Add("True");
            distractedItems!.Add("False");
            return Shuffle(distractedItems.Distinct().ToList());
        }
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
            var operation = (new Random().Next(8)) switch
            {
                <= 2 => Op.Greater,
                <= 4 => Op.Smaller,
                <= 6 => Op.Sub,
                _  => Op.Add,
            };
            var (operand1, operand2, answer) = OperandGenerator(operation, seed);
            yield return new Game { Operands = (operand1, operand2), Answer = answer, Op = operation, Options = AnswerOptions(answer) };   
        }
    }
}