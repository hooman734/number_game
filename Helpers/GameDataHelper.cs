namespace number_game.Helpers;

public class GameDataHelper
{
    public enum Op
    {
        Add = 1,
        Sub = 2
    }

    private readonly int _numberOfQuestions = 3;
    private readonly int _limit = 20;
    private readonly int _numberOfDistractedAnswers = 4;
    private static int _generalSeed = DateOnly.FromDateTime(DateTime.Now).DayNumber;

    private (int, int, int) OperandGenerator(Op op, int seed)
    {
        var rnd = new Random(_generalSeed + (int) op + seed);
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
        var rnd = new Random(_generalSeed);
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

    public IEnumerable<((int, int, int), Op, List<int>)>? MakeQuestion()
    {
        foreach (var questionItem in Enumerable.Range(0, _numberOfQuestions))
        {
            var seed = (new Random()).Next(20);
            var operation = (new Random()).Next(2) > 0 ? Op.Add : Op.Sub;
            yield return (OperandGenerator(operation, seed), operation, AnswerOptions(OperandGenerator(operation, seed).Item3));
        }
    }
}