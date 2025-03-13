/*
    Using instructions from https://www.youtube.com/watch?v=nnDNeJX-uBE 

    1. You need to create a Math game containing the 4 basic operations
    2. The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
    3. Users should be presented with a menu to choose an operation
    4. You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
*/

using System.Diagnostics;
using MathGameChallange;

MathGameLogic mathGame = new MathGameLogic();
Random random = new Random();

int firstNumber;
int secondNumber;
int userMenuSelection;
int score = 0;
bool gameOver = false;

DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

while (!gameOver)
{
    userMenuSelection = GetUserMenuSelection(mathGame);

    firstNumber = random.Next(1, 101); // random number between 1 and 100, 101 is exclusive
    secondNumber = random.Next(1, 101);

    switch (userMenuSelection)
    {
        case 1:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
            break;

        case 2:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '-', difficultyLevel);
            break;

        case 3:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '*', difficultyLevel);
            break;

        case 4:
            while (firstNumber % secondNumber != 0) // if the first number is not divisible by the second number, we will keep generating a new number until it is
            {
                firstNumber = random.Next(1, 101);
                secondNumber = random.Next(1, 101);
            }
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '/', difficultyLevel);
            break;

        case 5:
            int numberOfQuestions = 99;
            Console.WriteLine("How many questions would you like to answer?");
            while (!int.TryParse(Console.ReadLine(), out numberOfQuestions))
            {
                Console.WriteLine("Please enter a valid number of questions");
            }
            while (numberOfQuestions > 0)
            {
                int randomOperation = random.Next(1, 5);

                if (randomOperation == 1)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
                }
                else if (randomOperation == 2)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '-', difficultyLevel);
                }
                else if (randomOperation == 3)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '*', difficultyLevel);
                }
                else
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    while (firstNumber % secondNumber != 0)
                    {
                        firstNumber = random.Next(1, 101);
                        secondNumber = random.Next(1, 101);
                    }
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '/', difficultyLevel);
                }
                numberOfQuestions--; // decrement the number of questions till it reaches 0
            }
            break;
        case 6:
            Console.WriteLine("Game History: \n");
            foreach (var operation in mathGame.GameHistory)
            {
                Console.WriteLine($"{operation}");
            }
            break;

        case 7:
            difficultyLevel = ChangeDifficulty();
            DifficultyLevel difficultyEnum = (DifficultyLevel)difficultyLevel;
            Enum.IsDefined(typeof(DifficultyLevel), difficultyEnum);
            Console.WriteLine($"Difficulty level changed to {difficultyLevel}");
            break;

        case 8:
            gameOver = true;
            Console.WriteLine($"Game Over! Your final score is {score}");
            break;
    }
}


static DifficultyLevel ChangeDifficulty()
{
    int userSelection = 0;

    Console.WriteLine("Please select the difficulty level:");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");

    while (!int.TryParse(Console.ReadLine(), out userSelection) || (userSelection < 1 || userSelection > 3))
    {
        Console.WriteLine("Please enter a valid option 1-3");
    }

    switch (userSelection)
    {
        case 1:
            return DifficultyLevel.Easy;
        case 2:
            return DifficultyLevel.Medium;
        case 3:
            return DifficultyLevel.Hard;

    }

    return DifficultyLevel.Easy; // if something, video 39:40

    /* Other option for the switch case above:

    return userSelection switch
    {
        1 => DifficultyLevel.Easy,
        2 => DifficultyLevel.Medium,
        3 => DifficultyLevel.Hard,
        _ => DifficultyLevel.Easy,
    };
     */

}

static void DisplayMathGameQuestion(int firstNumber, int secondNumber, char operation)
{
    Console.WriteLine($"What is {firstNumber} {operation} {secondNumber}?");
}

static int GetUserMenuSelection(MathGameLogic mathGame)
{
    int selection = -1;
    mathGame.ShowMenu();
    while (selection < 1 || selection > 8)
    {
        while (!int.TryParse(Console.ReadLine(), out selection))
        {
            Console.WriteLine("Please enter a valid option 1-8");
        }


        if (!(selection >= 1 && selection <= 8))
        {
            Console.WriteLine("Please enter a valid option 1-8");
        }
    }
    return selection;
}


static async Task<int?> GetUserResponse(DifficultyLevel difficulty)
{
    int response = 0;
    int timeout = (int)difficulty; // casting that difficulty is int(45/30/15) and not string (easy/medium/hard)

    Stopwatch stopwatch = new Stopwatch(); // to record how long it takes for the user to respond
    stopwatch.Start();

    Task<string?> getUserInputTask = Task.Run(() => Console.ReadLine()); // task that will wait for user input

    try
    {
        string? result = await Task.WhenAny(getUserInputTask, Task.Delay(timeout * 1000)) == getUserInputTask ? getUserInputTask.Result : null; // if the user input is faster than the timeout, we will get the result, otherwise we will get null

        stopwatch.Stop();

        if (result != null && int.TryParse(result, out response)) // if the user input is not null and is a number, we will return the response
        {
            Console.WriteLine($"Time taken to answer: {stopwatch.Elapsed.ToString(@"mm\:ss\.fff")}"); // display the time taken to answer (needed to fix from origianl code)
            return response;
        }

        else
        {
            throw new OperationCanceledException();
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Time is up!");
        return null;
    }
}



static int ValidateResult(int result, int? userResponse, int score)
{
    if (result == userResponse)
    {
        Console.WriteLine("Congratulations, correct answer! 5 points to you!");
        score += 5;
    }
    else
    {
        Console.WriteLine("Incorrect! Try again!");
        Console.WriteLine($"The correct answer was {result}");
    }
    return score;
}




static async Task<int> PerformOperation(MathGameLogic mathGame, int firstNumber, int secondNumber, int score, char operation, DifficultyLevel difficulty)
{
    int result;
    int? userResponse;
    DisplayMathGameQuestion(firstNumber, secondNumber, operation);
    result = mathGame.MathOperation(firstNumber, secondNumber, operation);
    userResponse = await GetUserResponse(difficulty);
    score += ValidateResult(result, userResponse, score);
    return score;
}


public enum DifficultyLevel
{
    Easy = 45,
    Medium = 30,
    Hard = 15
};