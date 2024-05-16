// Auth: Ian
// Proj: Mini Golf Stats
// Desc: A lil command line app that kind of balooned to store and display minigolf stat
// Date: 5/14/24
// Ver1: 5/16/24
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Class declaration 
static class GolfStats
{

    // CONST and READONLY
    const float VERSION = 1.0f;
    const int LENGTH = 120;
    const int MINSTROKES = 1;
    const int MAXSTROKES = 10;
    const int DEFAULTNUMHOLES = 18;
    const string PROGRESS = "***";

    // FILE SAVING
    const string GOLFERSFILENAME = "golfers" + DATAEXTENSION;
    const string COURSESFILENAME = "courses" + DATAEXTENSION;
    const string DATAEXTENSION = ".dat";
    const string BACKUPPATH = "backup/";

    static readonly string DASHES = new string('-', 12);
    static readonly string CLEARSCREEN = new string('\n', 60);
    static readonly string UNDERSCORES = new string('_', LENGTH);
    static readonly string SPACES = new string(' ', LENGTH);



    static List<Golfer> golfers = new List<Golfer>();
    static List<Course> courses = new List<Course>();


    // Main Method 
    static void Main(string[] args)
    {

        //for (int i = 0; i < 10; i++) golfers.Add(new Golfer() { Name = "Ian" + i, ID = (uint)i });
        LoadData();


        // Main Loop
        bool exit = false;
        while (!exit)
        {
            PrintTitleCard("GolfStats V" + VERSION);
            PrintOptions(new List<string>() { "Exit", " GolfStats", "Golfers", "Courses"/*, " Leaderboards"*/ });
            int desiredAction = QueryIntRanged(0, 2, "Input an integer corrosponding to the desired action");

            if (desiredAction == 0) exit = true;
            else if (desiredAction == 1) ViewGolfers();
            else if (desiredAction == 2) ViewCourses();
        }

        Console.WriteLine(PROGRESS + " Exitting");
        SaveData();
        Console.WriteLine(PROGRESS + " Saving Complete");
    }
    public static void ViewGolfers()
    {
        PrintTitleCard("Golfers");
        PrintHeader("Navigation");

        // Generate Options
        List<string> options = new List<string>() { "Back", " Golfers", "Add Golfer" };
        for (int i = 0; i < golfers.Count; i++) options.Add(String.Format(
            "{0, -20} {1,6}% Over Par Per Hole",
            golfers[i].Name,
             GetParDefecit(golfers[i], DeficitType.AveragePercentOverPar).ToString("n2")
        ));

        // Print them
        PrintOptions(options);
        int desiredAction = QueryIntRanged(0, 1 + golfers.Count, "Input an integer corrosponding to the desired action");

        if (desiredAction == 0)
        {
            return;
        }
        else if (desiredAction == 1)
        {
            string name = QueryUniqueGolferName();

            // Get Unique Golfer ID
            uint uniqueID = 0;
            while (true)
            {
                // If Another matching Golfer Run it back
                bool success = true;
                foreach (Golfer golfer in golfers)
                    if (golfer.ID == uniqueID)
                    {
                        uniqueID++;
                        success = false;
                        break;
                    }

                // If ID was unique --> Break
                if (success) break;
            }

            golfers.Add(new Golfer() { Name = name, ID = uniqueID });

            Console.WriteLine("New Golfer created with name " + name);
        }
        else
        {
            ViewGolfer(golfers[desiredAction - 2]);
        }


        ViewGolfers();
    }
    public static void ViewGolfer(Golfer golfer)
    {
        PrintTitleCard(golfer.Name);

        // Print Stats
        PrintHeader("Stats");
        Console.WriteLine("Num States Played in: " + GetNumUniqueStatesPlayedIn(golfer));
        int matchesWon = GetNumMatches(golfer, true);
        int matchesLost = GetNumMatches(golfer, false);
        Console.WriteLine("                Wins: " + matchesWon);
        Console.WriteLine("              Losses: " + matchesLost);
        Console.WriteLine("         Win Percent: " + ((float)(matchesWon) / (matchesWon + matchesLost) * 100).ToString("n2") + "%");
        Console.Write("\n");
        Console.WriteLine("   Matches Completed: " + GetMatchesCompleted(golfer));
        Console.WriteLine("     Holes Completed: " + GetHolesCompleted(golfer));
        Console.Write("\n");
        Console.WriteLine(" Par Deficit (Total): " + GetParDefecit(golfer, DeficitType.Total));
        Console.WriteLine("Ave Over Par (Match): " + GetParDefecit(golfer, DeficitType.AveragePerGame).ToString("n2"));
        Console.WriteLine(" Ave Over Per (Hole): " + GetParDefecit(golfer, DeficitType.AveragePerHole).ToString("n2"));
        Console.Write("\n");
        Console.WriteLine("     Num Hole in One: " + GetHolesWithScore(golfer, 1));
        Console.WriteLine(" Percent Hole in One: " + ((float)GetHolesWithScore(golfer, 1) / GetHolesCompleted(golfer) * 100).ToString("n2") + "%");
        Console.Write("\n");
        Console.WriteLine("     Num Stroked Out: " + GetHolesWithScore(golfer, 10));
        Console.WriteLine(" Percent Stroked Out: " + ((float)GetHolesWithScore(golfer, 10) / GetHolesCompleted(golfer) * 100).ToString("n2") + "%");
        Console.Write("\n");
        NamedPlays mostPlayedCourse = GetMostPlayedCourse(golfer, true);
        NamedPlays mostPlayedLoop = GetMostPlayedCourse(golfer, false);
        Console.WriteLine("  Most Played Course: " + mostPlayedCourse.name + " played " + mostPlayedCourse.plays + " times");
        Console.WriteLine("    Most Played Loop: " + mostPlayedLoop.name + " played " + mostPlayedLoop.plays + " times");
        Console.Write("\n");
        List<string> favoriteCourses = GetFavoriteCourses(golfer);
        List<string> favoriteLoops = GetFavoriteLoops(golfer);
        Console.Write("  Favorite Course(s): ");
        foreach (string s in favoriteCourses) Console.WriteLine(s);
        Console.Write("    Favorite Loop(s): ");
        foreach (string s in favoriteLoops) Console.WriteLine(s);
        Console.Write("\n");
        MatchLoopCourse bestMatchLoopCourse = GetBestMatch(golfer);
        MatchLoopCourse worstMatchLoopCourse = GetBestMatch(golfer, false);
        PrintMatch(bestMatchLoopCourse.Match, bestMatchLoopCourse.Loop, worstMatchLoopCourse.Course, true, "Best Match");
        PrintMatch(worstMatchLoopCourse.Match, worstMatchLoopCourse.Loop, worstMatchLoopCourse.Course, true, "Worst Match");

        PrintHeader("Navigation");
        PrintOptions(new List<string>() { "Back", "All Reviews", " Danger Zone", "Change Name", "Delete" });
        int desiredAction = QueryIntRanged(0, 3, "Input an integer corrosponding to the desired action");

        if (desiredAction == 0)
        {
            return;
        }
        else if (desiredAction == 1)
        {
            ForEachMatchingReview(golfer, (course, loop, review) =>
            {
                string ratingStr = "Rating " + review.Rating.ToString("n2") + "/10 ";
                string difficultyStr = "Difficulty " + review.Difficulty.ToString("n2") + "/10 ";
                string nameLoop = course.Name + " in " + course.City + ", " + course.State + " " + loop.Name + " Loop";
                Console.WriteLine(ratingStr + difficultyStr + nameLoop);
            });
        }
        else if (desiredAction == 2)
        {
            string oldName = golfer.Name;
            string newName = QueryUniqueGolferName();

            golfer.Name = newName;

            Console.WriteLine("\n" + PROGRESS + "Golfer name Updated from " + oldName + " to " + newName);
        }
        else if (QueryTrueFalse("Are you sure you want to delete this Golfer?") && QueryTrueFalse("Are you sure? Deleting this Golfer will delete all associated Matches and Reviews!!!"))
        {
            // Remove Golfer
            golfers.Remove(golfer);

            // Remove all matches and reviews referencing this golfer
            foreach (Course c in courses)
                foreach (Loop loop in c.Loops)
                {
                    // Remove Reviews
                    for (int i = loop.Reviews.Count - 1; i >= 0; i--)
                        if (loop.Reviews[i].GolferID == golfer.ID)
                            loop.Reviews.RemoveAt(i);
                    // Remove Matches including this Golfer
                    for (int i = loop.Matches.Count - 1; i >= 0; i--)
                        if (loop.Matches[i].IsGolferPresent(golfer))
                            loop.Matches.RemoveAt(i);
                }

            Console.WriteLine(PROGRESS + "Golfer " + golfer.Name + " and all associated data has been DELETED. if this was not intended exit the application w/out saving!!!");

            return;
        }


        ViewGolfer(golfer);
    }
    public static void ViewCourses()
    {
        PrintTitleCard("Golf Courses");
        Console.WriteLine("");
        PrintHeader("Stats");
        Console.WriteLine("Hardest Hole: " + GetHardestHole().GetHoleString());
        Console.WriteLine("Easiest Hole: " + GetHardestHole(false).GetHoleString());
        Console.WriteLine("");
        PrintHeader("Navigation");

        // Generate Options
        List<string> options = new List<string>() { "Back", " Courses", "Add Golf Course" };
        for (int i = 0; i < courses.Count; i++) options.Add(
            string.Format("{0, -50} Rating: {1,4}/10  Difficulty: {2,4}/10  Ave Over Par: {3,5}%",
                courses[i].Name + " in " + courses[i].City + ", " + courses[i].State,
                courses[i].GetCumulativeRating().ToString("n2"),
                courses[i].GetCumulativeDifficulty().ToString("n2"),
                courses[i].GetAveragePercentOverPar().ToString("n2"))
            );

        // Print Options
        PrintOptions(options);
        int desiredAction = QueryIntRanged(0, courses.Count + 1, "Input an integer corrosponding to the desired action");


        if (desiredAction == 0)
        {
            return;
        }
        else if (desiredAction == 1)
        {
            // Get Course Name
            string name = QueryString("Enter the name of the Course");

            // Get Course City
            string city = CapitalizeFirstLetter(QueryString("Enter the City the course resides in"));

            // Get Course State
            State state = QueryState("Enter the State the Course resides in");


            courses.Add(new Course() { Name = name, City = city, State = state, Loops = new List<Loop>() });

            Console.WriteLine(PROGRESS + " New Course added  " + name + " in " + city + ", " + state);
        }
        else ViewCourse(courses[desiredAction - 2]);

        ViewCourses();
    }
    public static void ViewCourse(Course course)
    {
        PrintTitleCard(course.GetLongName());

        // Print Stats
        PrintHeader("Stats");
        Console.WriteLine("    Cumulative Rating: " + course.GetCumulativeRating());
        Console.WriteLine("Cumulative Difficulty: " + course.GetCumulativeDifficulty());
        Console.WriteLine("");
        Console.WriteLine("       Average Strokes: " + course.GetAverageStrokes());
        Console.WriteLine("        Total Strokes: " + course.GetTotalStrokes());
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("       Matches Played: " + course.GetTotalMatches());
        Console.WriteLine("");
        Console.WriteLine("         Hole in Ones: " + course.GetHolesWithScore(1));
        Console.WriteLine(" Percent Hole in Ones: " + course.GetPercentHolesWithScore(1).ToString("n2") + "%");
        Console.WriteLine("");
        Console.WriteLine("          Stroke Outs: " + course.GetHolesWithScore(10));
        Console.WriteLine("  Percent Stroke Outs: " + course.GetPercentHolesWithScore(10).ToString("n2") + "%");
        Console.WriteLine("");
        Console.WriteLine("         Hardest Hole: " + course.GetHardestHole().GetHoleString());
        Console.WriteLine("         Easiest Hole: " + course.GetHardestHole(false).GetHoleString());
        Console.WriteLine("");


        PrintHeader("Navigation");

        // Generate Options
        List<string> options = new List<string>() { "Back", " Loops", "Add Loop" };
        for (int i = 0; i < course.Loops.Count; i++) options.Add(String.Format(
            "{0,-22} Rating: {1,4}/10  Difficulty: {2,4}/10  Ave Over Par: {3,5}%  ",
            course.Loops[i].Name + " Loop",
            course.Loops[i].GetCumulativeRating().ToString("n2"),
            course.Loops[i].GetCumulativeDifficulty().ToString("n2"),
            course.Loops[i].GetAveragePercentOverPar().ToString("n2")
        ));
        options.Add(" Danger Zone");
        options.Add("Edit");
        options.Add("Delete");

        // ACTIONS
        PrintOptions(options);
        int desiredAction = QueryIntRanged(0, course.Loops.Count + 3, "Input an integer corrosponding to the desired action");

        if (desiredAction == 0) { return; }
        if (desiredAction == 1)
        {
            // Get Loop Unique Loop Name
            string loopName = "";
            while (true)
            {
                loopName = QueryString("Enter New Loop Name");

                // Break when a unique name is returned
                if (course.GetLoop(loopName) == null) break;
                Console.Write("Name is taken, ");
            }

            int par = QueryIntRanged(1, 117, "Par");

            Loop newLoop = new Loop() { Name = loopName, NumHoles = DEFAULTNUMHOLES, Par = par, Matches = new List<Match>(), Reviews = new List<Review>() };
            course.Loops.Add(newLoop);

            Console.WriteLine("Created new Loop in " + course.Name + " called " + loopName + " with a par of " + par + " and " + newLoop.NumHoles + " holes.");
        }
        else if (desiredAction <= 1 + course.Loops.Count) ViewLoop(course, course.Loops[desiredAction - 2]);
        else if (desiredAction == course.Loops.Count + 2)
        {
            // Get Course Name
            string oldName = course.Name;
            course.Name = QueryString("Enter course name (prev: " + oldName + ")");

            // Get Course State
            State oldState = course.State;
            course.State = QueryState("Enter state (prev: " + oldState + ")");


            // Get Course City
            string oldCity = course.City;
            course.City = QueryString("Enter the new city (prev: " + oldCity + ")");


            Console.WriteLine(PROGRESS + " Name: " + oldName + " --> " + course.Name);
            Console.WriteLine(PROGRESS + " State: " + oldState + " --> " + course.State);
            Console.WriteLine(PROGRESS + " City: " + oldCity + " --> " + course.City);
        }
        else if (desiredAction == course.Loops.Count + 3 && QueryTrueFalse("Are you sure you want to delete " + course.Name + " in " + course.City + ", " + course.State) && QueryTrueFalse("This will delete all associated loops, matches, and reviews. ARE YOU SURE??"))
        {
            courses.Remove(course);
            PrintTitleCard(course.Name + ", " + course.City + ", " + course.State + "and all associated data has been DELETED. If this was not intended exit the application w/out saving!!!");
            return;
        }


        // View this Course again if action didnt break
        ViewCourse(course);
    }
    public static void ViewLoop(Course course, Loop loop)
    {
        // General Info
        PrintTitleCard(course.GetLongName() + " - " + loop.Name + " Loop");

        // Print Stats
        PrintHeader("Stats");
        Console.WriteLine("    Cumulative Rating: " + loop.GetCumulativeRating());
        Console.WriteLine("Cumulative Difficulty: " + loop.GetCumulativeDifficulty());
        Console.Write("\n");
        Console.WriteLine("                Holes: " + loop.NumHoles);
        Console.WriteLine("                  Par: " + loop.Par);
        Console.WriteLine("");
        Console.WriteLine("       Average Strokes: " + loop.GetAverageStrokes());
        Console.WriteLine("        Total Strokes: " + loop.GetTotalStrokes());
        Console.WriteLine("");
        Console.WriteLine("       Matches Played: " + loop.Matches.Count);
        Console.WriteLine("");
        Console.WriteLine("         Hole in Ones: " + loop.GetHolesWithScore(1));
        Console.WriteLine(" Percent Hole in Ones: " + loop.GetPercentHolesWithScore(1).ToString("n2") + "%");
        Console.WriteLine("");
        float[] averageStrokesPerHole = loop.GetAverageStrokePerHole();
        Console.WriteLine("          Stroke Outs: " + loop.GetHolesWithScore(10));
        Console.WriteLine("  Percent Stroke Outs: " + loop.GetPercentHolesWithScore(10).ToString("n2") + "%");
        Console.WriteLine("");
        HoleInfo hardestHole = loop.GetHardestHole();
        HoleInfo easiestHole = loop.GetHardestHole(false);
        Console.WriteLine("         Hardest Hole: " + hardestHole.GetHoleString());
        Console.WriteLine("         Easiest Hole: " + easiestHole.GetHoleString());
        // Add init Nums
        Console.Write("\n     Average Strokes:\n        Hole: ");
        for (int i = 0; i < loop.NumHoles; i++) Console.Write(String.Format("{0,-6}", i + 1));
        Console.Write("\n     Average: ");
        for (int i = 0; i < averageStrokesPerHole.Length; i++) Console.Write(String.Format("{0,-6}", averageStrokesPerHole[i].ToString("n1")));
        Console.Write("\n\n");
        PrintHeader("Navigation");

        // Generate Options
        List<string> options = new List<string>() { "Back", " Reviews", "View Reviews", "Add Review", "Edit Review", "Delete Review", " Matches", "Add Match" };
        for (int i = 0; i < loop.Matches.Count; i++) // Match Options
        {
            string matchString = "";
            foreach (ScoreCard card in loop.Matches[i].ScoreCards)
            {
                Golfer thisGolfer = GetGolfer(card.GolferID);
                matchString += GetGolferMatchName(loop.Matches[i], thisGolfer) + " v ";
            }
            matchString = matchString.Substring(0, matchString.Length - 2);
            matchString += loop.Matches[i].Date.ToString("d");
            options.Add(matchString);
        }
        if (loop.Matches.Count != 0) options.Add("");
        options.Add(" Danger Zone");
        options.Add("Edit");
        options.Add("Delete");

        // Print Options and Query for selection
        PrintOptions(options);
        int desiredAction = QueryIntRanged(0, 7 + loop.Matches.Count, "Input an integer corrosponding to the desired action");

        // Actions
        if (desiredAction == 0) { return; }
        else if (desiredAction == 1)
        {
            for (int i = 0; i < loop.Reviews.Count; i++)
                Console.WriteLine(String.Format("{0} gave a Rating of {1,3}/10 and a Difficulty of {2,3}/10", GetGolfer(loop.Reviews[i].GolferID).Name, loop.Reviews[i].Rating, loop.Reviews[i].Difficulty));
        }
        else if (desiredAction == 2)
        {
            uint golferID = QueryGolfer().ID;

            bool alreadyRated = false;
            foreach (Review review in loop.Reviews) if (review.GolferID == golferID) alreadyRated = true;

            if (alreadyRated)
            {
                Console.WriteLine(GetGolfer(golferID).Name + " has already rated this loop!");
            }
            else
            {
                float rating = GetFloatInRange(0, 10, "Enter an overall rating");
                float difficulty = GetFloatInRange(0, 10, "Enter a difficulty");

                loop.Reviews.Add(new Review() { GolferID = golferID, Rating = rating, Difficulty = difficulty });

                Console.WriteLine("Created new Review for " + loop.Name + " with a rating of " + rating + " and a difficulty of " + difficulty + "!");
            }
        }
        else if (desiredAction == 3)
        {
            if (loop.Reviews.Count != 0)
            {
                Review reviewToEdit = loop.GetReview(QueryGolfer().ID);

                // Get new Review
                float oldRating = reviewToEdit.Rating;
                reviewToEdit.Rating = GetFloatInRange(0, 10, "Enter an overall rating (was " + oldRating + ")");

                // Get new Difficulty
                float oldDifficulty = reviewToEdit.Difficulty;
                reviewToEdit.Difficulty = GetFloatInRange(0, 10, "Enter a difficulty (was " + reviewToEdit.Difficulty + ")");


                Console.WriteLine(PROGRESS + " Review: " + oldRating + " --> " + reviewToEdit.Rating);
                Console.WriteLine(PROGRESS + " Difficulty: " + oldDifficulty + " --> " + reviewToEdit.Difficulty);
            }
        }
        else if (desiredAction == 4)
        {
            if (loop.Reviews.Count != 0)
            {
                Review reviewToDelete = loop.GetReview(QueryGolfer().ID);

                loop.Reviews.Remove(reviewToDelete);
                Console.WriteLine(PROGRESS + " Deleted Review");
            }
        }
        else if (desiredAction == 5)
        {
            // Get Date
            DateTime date = QueryDate("Enter the match's");

            // Build Score Cards
            List<ScoreCard> scoreCards = new List<ScoreCard>();
            while (true)
            {
                // Allow Breaking after two score cards added!
                if (scoreCards.Count >= 2 && !QueryTrueFalse("Add another score card"))
                    break;

                // Query and add new score cards
                string query = "Enter the name of golfer " + (scoreCards.Count + 1);
                ScoreCard scoreCard = QueryScoreCard(loop.NumHoles, query);
                scoreCards.Add(scoreCard);

                // Note to User
                Console.WriteLine(PROGRESS + " Added scorecard for " + GetGolfer(scoreCard.GolferID).Name);
            }

            loop.Matches.Add(new Match() { Date = date, ScoreCards = scoreCards });

            Console.WriteLine("Created new Match on " + date.ToString("d") + " with " + scoreCards.Count + " scorecards. ");
        }
        else if (desiredAction <= 5 + loop.Matches.Count)
        {
            ViewMatch(loop.Matches[desiredAction - 6], course, loop);
        }
        else if (desiredAction == 6 + loop.Matches.Count)
        {
            // Get new Loop Name
            string oldName = loop.Name;
            loop.Name = QueryString("Enter new loop name (prev: " + oldName + ")");

            // Get new Par
            int oldPar = loop.Par;
            loop.Par = QueryIntRanged(1, 117, "Enter new Par (prev: " + oldPar + ")");


            Console.WriteLine(PROGRESS + " Name: " + oldName + " --> " + loop.Name);
            Console.WriteLine(PROGRESS + " Par: " + oldPar + " --> " + loop.Par);
        }
        else if (desiredAction == 7 + loop.Matches.Count &&
            QueryTrueFalse("Are you sure you want to delete the " + loop.Name + " loop") &&
            QueryTrueFalse("This will delete " + loop.Matches.Count + " associated matches and " + loop.Reviews.Count + " associated reviews, Are you sure??"))
        {
            // Delete the loop
            foreach (Course courseToDelete in courses)
                for (int i = 0; i <= courseToDelete.Loops.Count; i++)
                    if (courseToDelete.Loops[i] == loop)
                    {
                        courseToDelete.Loops.RemoveAt(i);

                        Console.WriteLine(PROGRESS + " " + loop.Name + " loop DELETED along with " + loop.Matches.Count + " matches and " + loop.Reviews.Count + " reviews");
                        return;
                    }
        }
        // View this loop again if action didnt break
        ViewLoop(course, loop);
    }
    public static void ViewMatch(Match match, Course course, Loop loop)
    {
        //Print Info
        PrintTitleCard("Match on " + loop.Name + " Loop at " + course.GetLongName() + " with " + match.ScoreCards.Count + " players on " + match.Date.ToString("d"));
        PrintMatch(match, loop, course);
        PrintHeader("Navigation");

        // Show and Get Actions
        PrintOptions(new List<string> { "Back", " Scorecard", "Edit Strokes", "Add Golfer", "Remove Golfer", " Danger Zone", "Change Date", "Delete" });
        int desiredAction = QueryIntRanged(0, 5);



        // Actions
        if (desiredAction == 0) { return; }
        if (desiredAction == 1)
        {
            ScoreCard toEdit = match.GetScoreCard(QueryGolfer().ID);
            int holeToEdit = QueryIntRanged(1, loop.NumHoles, "Enter the hole to edit");
            int oldStrokes = toEdit.Strokes[holeToEdit - 1];
            toEdit.Strokes[holeToEdit - 1] = QueryIntRanged(MINSTROKES, MAXSTROKES, "Enter the new number of strokes for hole " + holeToEdit + " (was: " + oldStrokes + ")");

            Console.WriteLine(PROGRESS + " Strokes for hole " + holeToEdit + ": " + oldStrokes + " --> " + toEdit.Strokes[holeToEdit - 1]);
        }
        if (desiredAction == 2)
        {
            // Generate ScoreCard
            ScoreCard scoreCard = QueryScoreCard(loop.NumHoles);

            // Add it
            match.ScoreCards.Add(scoreCard);

            // Inform user
            string golferName = GetGolfer(scoreCard.GolferID).Name;
            Console.WriteLine(PROGRESS + " Added score card for " + golferName);
        }
        if (desiredAction == 3)
        {
            if (match.ScoreCards.Count <= 2)
            {
                Console.WriteLine(PROGRESS + " Cannot Remove a Golfer from a match when there are only two Golfers");
            }
            else
            {
                ScoreCard toDelete = match.GetScoreCard(QueryGolfer().ID);
                string golferName = GetGolfer(toDelete.GolferID).Name;
                if ((QueryTrueFalse("Are you sure you want to delete " + golferName + "'s score for this match")))
                {
                    Console.WriteLine(PROGRESS + " " + golferName + "'s score DELETED for this match");
                }
            }
        }
        if (desiredAction == 4)
        {
            // Get new Date
            DateTime oldDate = match.Date;
            match.Date = QueryDate("Enter the match's new");

            Console.WriteLine(PROGRESS + " Date: " + oldDate.ToString("d") + " --> " + match.Date.ToString("d"));
        }
        if (desiredAction == 5 && QueryTrueFalse("Are you sure you want to delete this Match") && QueryTrueFalse("This will delete the entire match and all history of it, Are you sure"))
        {
            loop.Matches.Remove(match);
            PrintTitleCard("Match on the " + loop.Name + " loop on " + match.Date.ToString("d") + " has been DELETED. If this was not intended exit the application w/out saving!!!");
            return;
        }


        ViewMatch(match, course, loop);
    }


    public static void ForEachMatchingReview(Golfer golfer, Action<Course, Loop, Review> action)
    {
        foreach (Course course in courses)
            foreach (Loop loop in course.Loops)
                foreach (Review review in loop.Reviews)
                    if (review.GolferID == golfer.ID)
                        action.Invoke(course, loop, review);
    }
    public static void ForEachMatch(Action<Course, Loop, Match> action)
    {
        foreach (Course course in courses)
            foreach (Loop loop in course.Loops)
                foreach (Match match in loop.Matches)
                    action.Invoke(course, loop, match);
    }
    public static void ForEachScorecard(Action<Course, Loop, Match, ScoreCard> action)
    {
        foreach (Course course in courses)
            foreach (Loop loop in course.Loops)
                foreach (Match match in loop.Matches)
                    foreach (ScoreCard scoreCard in match.ScoreCards)
                        action.Invoke(course, loop, match, scoreCard);
    }
    public static void ForEachMatchingScorecard(Action<Course, Loop, Match, ScoreCard> action, Golfer golfer)
    {
        ForEachScorecard((course, loop, match, scoreCard) => { if (scoreCard.GolferID == golfer.ID) action.Invoke(course, loop, match, scoreCard); });
    }
    public static int GetMatchesCompleted(Golfer golfer)
    {
        int sum = 0;
        ForEachMatchingScorecard((course, loop, match, scoreCard) => sum += 1, golfer);
        return sum;
    }
    public static int GetHolesCompleted(Golfer golfer)
    {
        int sum = 0;
        ForEachMatchingScorecard((course, loop, match, scoreCard) => sum += scoreCard.Strokes.Length, golfer);
        return sum;
    }
    public static int GetHolesWithScore(Golfer golfer, int score)
    {
        int sum = 0;
        ForEachMatchingScorecard((course, loop, match, scoreCard) => { sum += scoreCard.GetHolesWithScore(score); }, golfer);
        return sum;
    }
    public static int GetNumMatches(Golfer golfer, bool won)
    {
        int sum = 0;
        ForEachMatch((course, loop, match) => { if (match.DidWin(golfer) == won) sum++; });
        return sum;
    }
    public static int GetNumUniqueStatesPlayedIn(Golfer golfer)
    {
        List<State> uniqueStates = new List<State>();

        ForEachMatch((course, loop, match) =>
        {
            if (match.IsGolferPresent(golfer) && !uniqueStates.Contains(course.State))
            {
                uniqueStates.Add(course.State);
            }
        });

        return uniqueStates.Count;
    }
    public struct NamedPlays
    {
        public string name;
        public int plays;
    }
    public class MatchLoopCourse
    {
        public Match Match { get; set; }
        public Loop Loop { get; set; }
        public Course Course { get; set; }
    }
    public class RatingCountPair
    {
        public float ratingSum = 0;
        public int count = 1;

        public float GetCumulativeRating() { return ratingSum / count; }
    }
    public static MatchLoopCourse GetBestMatch(Golfer golfer, bool getBestMatch = true)
    {
        MatchLoopCourse best = new MatchLoopCourse();
        float bestRelativeParDefecit = getBestMatch ? float.MaxValue : float.MinValue; // Par defecit relative to the loop's Par
        ForEachMatchingScorecard((course, loop, match, scoreCard) =>
        {
            float relativeParDefecit = (float)(scoreCard.GetTotalScore() - loop.Par) / loop.Par;
            //Console.WriteLine(relativeParDefecit + " < " + bestRelativeParDefecit);
            if ((relativeParDefecit < bestRelativeParDefecit) == getBestMatch)
            {
                best = new MatchLoopCourse() { Match = match, Loop = loop, Course = course };
                bestRelativeParDefecit = relativeParDefecit;
            }
        }, golfer);


        return best;
    }
    public static List<string> GetFavoriteCourses(Golfer golfer)
    {
        // Build Dictionary of Ratings by this user for each course
        Dictionary<Course, RatingCountPair> courseRatings = new Dictionary<Course, RatingCountPair>();
        ForEachMatchingReview(golfer, (course, loop, review) =>
                {
                    if (courseRatings.ContainsKey(course))
                    {
                        courseRatings[course].ratingSum += review.Rating;
                        courseRatings[course].count++;
                    }
                    else courseRatings.Add(course, new RatingCountPair() { ratingSum = review.Rating, count = 1 });
                });



        // Find the Highest Rated Courses (By this user)
        float highestRating = 0;
        List<string> highestRatedCourses = new List<string>() { "None" };
        foreach (KeyValuePair<Course, RatingCountPair> pair in courseRatings)
        {
            // Better rating 
            if (pair.Value.GetCumulativeRating() > highestRating)
            {
                highestRating = pair.Value.GetCumulativeRating();
                highestRatedCourses.Clear();
                highestRatedCourses.Add(pair.Key.GetLongName());
            }
            // Equal Rating add it to list of highest rated courses
            else if (pair.Value.GetCumulativeRating() == highestRating)
            {
                highestRatedCourses.Add(pair.Key.GetLongName());
            }
        }


        return highestRatedCourses;
    }
    public static List<string> GetFavoriteLoops(Golfer golfer)
    {
        // Collect all courses with the highest rating
        List<string> highestRatedLoops = new List<string>() { "None" };
        float highestRating = 0;


        ForEachMatchingReview(golfer, (course, loop, review) =>
        {
            string name = loop.Name + " loop at " + course.GetLongName();

            // Better rating 
            if (review.Rating > highestRating)
            {
                highestRating = review.Rating;
                highestRatedLoops.Clear();
                highestRatedLoops.Add(name);

            }
            // Equal Rating add it to list of highest rated courses
            else if (review.Rating == highestRating)
            {
                highestRatedLoops.Add(name);
            }
        });


        return highestRatedLoops;
    }
    public static NamedPlays GetMostPlayedCourse(Golfer golfer, bool courseRatherThanLoop)
    {
        Dictionary<string, int> timesPlayed = new Dictionary<string, int>();
        ForEachMatchingScorecard((course, loop, match, scorecard) =>
        {
            string name;
            if (courseRatherThanLoop) name = course.GetLongName();
            else name = loop.Name + " loop at " + course.Name;

            if (timesPlayed.ContainsKey(name)) timesPlayed[name]++;
            else timesPlayed.Add(name, 1);
        }, golfer);


        NamedPlays mostPlayedCourse = new NamedPlays() { name = "None", plays = 0 };
        foreach (KeyValuePair<string, int> pair in timesPlayed)
        {
            if (pair.Value > mostPlayedCourse.plays)
            {
                mostPlayedCourse.name = pair.Key;
                mostPlayedCourse.plays = pair.Value;
            }
        }


        return mostPlayedCourse;
    }
    public enum DeficitType { Total, AveragePerGame, AveragePerHole, AveragePercentOverPar }
    public static float GetParDefecit(Golfer golfer, DeficitType type)
    {
        int totalDeficit = 0;
        int numGames = 0;
        int numHoles = 0;
        int totalPar = 0;
        ForEachMatchingScorecard((course, loop, match, scoreCard) => { totalDeficit += scoreCard.GetTotalScore() - loop.Par; numGames++; numHoles += loop.NumHoles; totalPar += loop.Par; }, golfer);

        if (type == DeficitType.Total) return totalDeficit;
        else if (type == DeficitType.AveragePerGame) return (float)totalDeficit / numGames;
        else if (type == DeficitType.AveragePerHole) return (float)totalDeficit / numHoles;
        else if (type == DeficitType.AveragePercentOverPar) return ((float)totalDeficit / totalPar * 100);

        throw new System.Exception("Invalid Deficit Type");
    }
    public static Golfer GetGolfer(uint id)
    {
        foreach (Golfer g in golfers)
            if (g.ID == id)
                return g;

        return null;
    }
    public static Golfer GetGolfer(string name)
    {
        foreach (Golfer g in golfers)
            if (name.ToLower().Equals(g.Name.ToLower()))
                return g;

        return null;
    }
    public static HoleInfo GetHardestHole(bool getHardest = true)
    {
        // Cache Hardest Hole and its Average strokes
        HoleInfo bestHole = new HoleInfo() { AverageStrokes = getHardest ? float.MinValue : float.MaxValue, HoleNum = -1 };

        // Go Through all Loops
        foreach (Course course in courses)
            foreach (Loop loop in course.Loops)
            {
                float[] averageStrokesPerHole = loop.GetAverageStrokePerHole();
                for (int i = 0; i < averageStrokesPerHole.Length; i++)
                    if ((averageStrokesPerHole[i] > bestHole.AverageStrokes) == getHardest)
                        bestHole = new HoleInfo() { Course = course, Loop = loop, HoleNum = i + 1, AverageStrokes = averageStrokesPerHole[i] };
            }


        return bestHole;
    }
    public static bool CourseNameTaken(string name)
    {
        foreach (Course c in courses) if (c.Name.Equals(name)) return true;
        return false;
    }
    public static string GetGolferMatchName(Match match, Golfer golfer)
    {
        // Returns the name of the Golfer
        // Spruces up the Name if they won the match
        return match.DidWin(golfer) ? "(W)" + golfer.Name.ToUpper() : golfer.Name;
    }

    // The following methods get some var using user input
    // - query repeatedly until valid value is found
    public static ScoreCard QueryScoreCard(int numHoles, string query = "Enter the name of the golfer")
    {
        uint golferID = QueryGolfer(query).ID;
        int[] strokes = new int[numHoles];

        for (int i = 0; i < numHoles; i++)
            strokes[i] = QueryIntRanged(MINSTROKES, MAXSTROKES, "Strokes for hole " + (i + 1) + ": ");

        return new ScoreCard() { GolferID = golferID, Strokes = strokes };
    }
    public static DateTime QueryDate(string query)
    {
        // Get Date
        int year = QueryIntRanged(0, 9999, query + " year");
        int month = QueryIntRanged(1, 12, query + " month");
        int day = QueryIntRanged(1, GetDaysInMonth(year, month), query + " day");

        return new DateTime(year, month, day);
    }
    public static State QueryState(string query)
    {
        // Querys the user for one of the great american states 07 \\>
        while (true)
        {
            string stateString = QueryString(query);
            try
            {
                // Make First letter Capital all others lower case
                stateString = char.ToUpper(stateString[0]) + stateString.Substring(1).ToLower();

                // Convert to a state
                return (State)Enum.Parse(typeof(State), stateString);
            }
            catch
            {
                Console.WriteLine("This state doesn't exist!\n");
            }
        }
    }
    public static string QueryUniqueGolferName()
    {
        string name = "";
        Console.Write("Enter a unique name for your golfer: ");
        while (true)
        {
            name = Console.ReadLine();
            if (GetGolfer(name) != null) { Console.Write("This name is taken!, Try again: "); continue; }

            // Trailing Break
            Console.Write("\n");

            return name;
        }
    }
    public static Golfer QueryGolfer(string query = "Enter the name of the desired golfer")
    {
        if (golfers.Count == 0) throw new System.Exception("Fatal Error, No Golfers!");
        Console.Write(query + ": ");

        while (true)
        {
            string txt = Console.ReadLine();
            Golfer golfer = GetGolfer(txt);
            if (golfer == null) { Console.Write("No Golfer with this name, Try again: "); continue; }

            // Trailing Break
            Console.Write("\n");

            // Success
            return golfer;
        }
    }
    public static string QueryString(string query = "Input a string")
    {
        Console.Write(query + ": ");

        while (true)
        {
            string txt = Console.ReadLine();
            if (txt.Equals("")) { Console.Write("Empty submission, Try again: "); continue; } // Outside Range

            // Trailing Break
            Console.Write("\n");

            // Success
            return txt;
        }
    }
    public static string QueryOptions(List<string> options, string query = "Enter the desired option")
    {
        if (options == null || options.Count == 0) throw new System.Exception("FAILED: Options Null or Empty");

        Console.Write(query + " (");
        foreach (string option in options) Console.Write(option + ", ");
        Console.WriteLine(")");

        while (true)
        {
            string selection = Console.ReadLine();

            foreach (string option in options) if (selection.ToLower().Equals(option.ToLower())) return option;

            Console.Write("Input must be one of the options, Try again: ");
            // Trailing Break
            Console.Write("\n");
        }
    }
    public static int QueryIntRanged(int min, int max, string query = "Input an integer")
    {
        if (min > max) throw new System.Exception("Min must be < Max");

        string range = "[" + min + ", " + max + "]";
        Console.Write(query + " " + range + ": ");
        while (true)
        {
            int value = 0;
            try { value = int.Parse(Console.ReadLine()); }
            catch { Console.Write("Not an Integer, Try again: "); continue; } // Invalid type
            if (value > max || value < min) { Console.Write("Value must be within " + range + ", Try again: "); continue; } // Outside Range

            // Trailing Break
            Console.Write("\n");

            // Success
            return value;
        }
    }
    public static float GetFloatInRange(float min, float max, string query = "Input a number")
    {
        if (min > max) throw new System.Exception("Min must be < Max");

        string range = "[" + min + ", " + max + "]";
        Console.Write(query + " " + range + ": ");
        while (true)
        {
            float value = 0;
            try { value = float.Parse(Console.ReadLine()); }
            catch { Console.Write("Not a Number, Try again: "); continue; } // Invalid type
            if (value > max || value < min) { Console.Write("Value must be within " + range + ", Try again: "); continue; } // Outside Range

            // Trailing Break
            Console.Write("\n");

            // Success
            return value;
        }
    }
    public static bool QueryTrueFalse(string query)
    {
        Console.Write(query + "? (y/n): ");

        while (true)
        {
            string txt = Console.ReadLine().ToLower();

            // Trailing Break
            Console.Write("\n");

            if (txt.Equals("y") || txt.Equals("yes")) return true;
            return false;
        }
    }

    public static void SaveData()
    {
        var serializer = new JavaScriptSerializer();


        // SAVE BACKUP
        // Create Backup Directory if it doesnt exist
        if (!Directory.Exists(BACKUPPATH))
            Directory.CreateDirectory(BACKUPPATH);
        // Move old files to backup
        if (File.Exists(GOLFERSFILENAME))
            File.Move(GOLFERSFILENAME, BACKUPPATH + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "-" + GOLFERSFILENAME);
        // Load Courses
        if (File.Exists(COURSESFILENAME))
            File.Move(COURSESFILENAME, BACKUPPATH + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "-" + COURSESFILENAME);


        // SAVE CURRENT DATA
        // Save Golfers
        var serializedResult = serializer.Serialize(golfers);
        File.WriteAllText(GOLFERSFILENAME, serializedResult);

        // Save Courses
        serializedResult = serializer.Serialize(courses);
        File.WriteAllText(COURSESFILENAME, serializedResult);
    }
    public static void LoadData()
    {
        var serializer = new JavaScriptSerializer();

        // Load Golfers
        if (File.Exists(GOLFERSFILENAME))
        {
            string text = File.ReadAllText(GOLFERSFILENAME);
            golfers = serializer.Deserialize<List<Golfer>>(text);
        }

        // Load Courses
        if (File.Exists(COURSESFILENAME))
        {
            string text = File.ReadAllText(COURSESFILENAME);
            courses = serializer.Deserialize<List<Course>>(text);
        }
    }



    public static void PrintOptions(List<string> options)
    {   // Prints passed options numbered by their index in the list
        // *** Empty options will be printed as a line break          their index passed to the next option
        // *** Options that start with a space are considered a title their index passed to the next option

        // Go through all Options
        int index = 0;
        foreach (string option in options)
        {
            if (option.Equals("")) Console.WriteLine("");
            else if (option[0] == ' ') { Console.WriteLine(""); PrintHeader(option.Substring(1)); }
            else Console.WriteLine("(" + (index++) + ") " + option);
        }
        // Print Line Break or the option


        // Trailing Line Break
        Console.Write("\n");
    }
    public static void PrintMatch(Match match, Loop loop, Course course, bool printTitle = false, string title = "Match Details")
    {   // Incase trying to print empty match
        if (match == null) return;

        int nameWidth = 20;


        // Match Title
        if (printTitle) Console.WriteLine(string.Format("{0," + nameWidth + "}: {1}", title, loop.Name + " Loop at " + course.GetLongName() + " on " + match.Date.ToString("d")));


        // Add init Nums
        Console.Write(string.Format("{0, " + nameWidth + "}: ", "Holes"));
        Console.Write(string.Format("{0, 5} ", "Total"));
        for (int i = 0; i < match.ScoreCards[0].Strokes.Length; i++) Console.Write(String.Format("{0,3}", i + 1));
        Console.WriteLine("");

        foreach (ScoreCard scoreCard in match.ScoreCards)
        {
            Golfer thisGolfer = GetGolfer(scoreCard.GolferID);
            Console.Write(string.Format("{0, " + nameWidth + "}: ", GetGolferMatchName(match, thisGolfer))); // Name
            Console.Write(string.Format("{0, 5} ", scoreCard.GetTotalScore()));                             // Total
            for (int i = 0; i < match.ScoreCards[0].Strokes.Length; i++) Console.Write(String.Format("{0,3}", scoreCard.Strokes[i])); // Strokes Per Hole
            Console.WriteLine("");
        }
        Console.WriteLine(string.Format("{0, " + nameWidth + "}: {1,5}", "Par", loop.Par));

        Console.WriteLine("");
    }
    public static void PrintNotImplemented()
    {
        Console.WriteLine(PROGRESS + "Not Implemented in V" + VERSION);

        //PrintTitleCard("Not Implemented in V" + VERSION);
    }
    public static void PrintTitleCard(string title)
    {
        Console.WriteLine(string.Format("{3} {0} \n|{1}|\n|{2}|\n|{0}|\n", UNDERSCORES, SPACES, GetCenteredString(title, LENGTH), CLEARSCREEN));
    }
    public static void PrintHeader(string txt)
    {
        Console.WriteLine(DASHES + " " + txt + " " + DASHES);
    }
    public static string GetCenteredString(string s, int width)
    {
        if (s.Length >= width)
        {
            return s;
        }

        int leftPadding = (width - s.Length) / 2;
        int rightPadding = width - s.Length - leftPadding;

        return new string(' ', leftPadding) + s + new string(' ', rightPadding);
    }
    public static int GetDaysInMonth(int year, int month)
    {
        if (month == 1) return 31;
        if (month == 2) return (year % 400 == 0 && (year % 4 == 0 && year % 100 != 0)) ? 29 : 28;
        if (month == 3) return 31;
        if (month == 4) return 30;
        if (month == 5) return 31;
        if (month == 6) return 30;
        if (month == 7) return 31;
        if (month == 8) return 31;
        if (month == 9) return 30;
        if (month == 10) return 31;
        if (month == 11) return 30;
        if (month == 12) return 31;

        throw new System.Exception("Month " + month + " is an invalid month!");
    }
    public static string CapitalizeFirstLetter(string input) { return char.ToUpper(input[0]) + input.Substring(1).ToLower(); }
}



public class Golfer
{
    public uint ID { get; set; }
    public string Name { get; set; }
}
public class Course
{
    public string GetLongName() { return Name + " in " + City + ", " + State; }

    public string Name { get; set; }
    public State State { get; set; }
    public string City { get; set; }
    public List<Loop> Loops { get; set; }


    public float GetAveragePar()
    {
        int sumPars = 0;
        foreach (Loop loop in Loops) sumPars += loop.Par;

        return (float)sumPars / Loops.Count;
    }
    public float GetCumulativeDifficulty()
    {
        float sumDifficulty = 0;
        int numReviewedLoops = 0;
        foreach (Loop loop in Loops)
        {   // Skip Loops with NaN rating
            float loopCumulativeDiff = loop.GetCumulativeDifficulty();
            if (float.IsNaN(loopCumulativeDiff)) continue;

            // Add Ratings to sum
            sumDifficulty += loopCumulativeDiff;
            numReviewedLoops++;
        }

        return sumDifficulty / numReviewedLoops;
    }
    public float GetCumulativeRating()
    {
        float sumRating = 0;
        int numReviewedLoops = 0;
        foreach (Loop loop in Loops)
        {   // Skip Loops with NaN rating
            float loopCumulativeRating = loop.GetCumulativeRating();
            if (float.IsNaN(loopCumulativeRating)) continue;

            // Add Ratings to sum
            sumRating += loopCumulativeRating;
            numReviewedLoops++;
        }

        return sumRating / numReviewedLoops;
    }
    public int GetTotalStrokes()
    {
        int sum = 0;
        foreach (Loop loop in Loops)
            sum += loop.GetTotalStrokes();

        return sum;
    }
    public float GetAverageStrokes()
    {
        if (Loops.Count == 0) return 0;


        int sum = 0;
        foreach (Loop loop in Loops)
            sum += loop.GetAverageStrokes();

        return sum / Loops.Count;
    }
    public int GetTotalMatches()
    {
        int sum = 0;
        foreach (Loop loop in Loops)
            sum += loop.Matches.Count;

        return sum;
    }
    public int GetHolesWithScore(int score)
    {
        int sum = 0;
        foreach (Loop loop in Loops)
            sum += loop.GetHolesWithScore(score);

        return sum;
    }
    public HoleInfo GetHardestHole(bool getHardest = true)
    {
        // Cache Hardest Hole and its Average strokes
        HoleInfo bestHole = new HoleInfo() { AverageStrokes = getHardest ? float.MinValue : float.MaxValue, HoleNum = -1 };

        // Go Through all Loops
        foreach (Loop loop in Loops)
        {
            float[] averageStrokesPerHole = loop.GetAverageStrokePerHole();
            for (int i = 0; i < averageStrokesPerHole.Length; i++)
                if ((averageStrokesPerHole[i] > bestHole.AverageStrokes) == getHardest)
                    bestHole = new HoleInfo() { Loop = loop, HoleNum = i + 1, AverageStrokes = averageStrokesPerHole[i] };
        }


        return bestHole;
    }
    public float GetAveragePercentOverPar()
    {
        float sumOfLoopPercents = 0;
        int numLoopPercents = 0;

        foreach (Loop loop in Loops)
        {
            // Loop has >0 Games Gate
            if (loop.Matches.Count <= 0) continue;

            numLoopPercents++;
            sumOfLoopPercents += loop.GetAveragePercentOverPar();
        }

        if (numLoopPercents == 0) return 0;
        return sumOfLoopPercents / numLoopPercents;
    }
    public float GetPercentHolesWithScore(int score)
    {
        float sumPercent = 0;
        int countedPercents = 0;

        // Count Loops that have matches and sum their percents
        foreach (Loop loop in Loops)
        {
            if (loop.Matches.Count <= 0) continue;

            sumPercent += loop.GetPercentHolesWithScore(score);
            countedPercents++;
        }

        return sumPercent / countedPercents;
    }
    public Loop GetLoop(string name)
    {
        foreach (Loop loop in Loops)
            if (loop.Name.Equals(name)) return loop;

        return null;
    }
}
public class Loop
{
    public string GetLongName() { return Name + " Loop"; }
    public string Name { get; set; }
    public int Par { get; set; }
    public int NumHoles { get; set; }
    public List<Review> Reviews { get; set; }
    public List<Match> Matches { get; set; }

    public int GetHolesCompleted()
    {
        int sum = 0;
        foreach (Match match in Matches) sum += match.ScoreCards.Count * NumHoles;
        return sum;
    }
    public int GetTotalStrokes()
    {
        int sum = 0;
        foreach (Match match in Matches)
            foreach (ScoreCard scoreCard in match.ScoreCards)
                sum += scoreCard.GetTotalScore();

        return sum;
    }
    public int GetAverageStrokes()
    {
        if (GetNumScoreCards() == 0) return 0;
        return GetTotalStrokes() / GetNumScoreCards();
    }
    public float GetAveragePercentOverPar()
    {
        return (float)(GetAverageStrokes() - Par) / Par * 100;
    }
    public float[] GetAverageStrokePerHole()
    {
        int[] totalStrokesPerHole = new int[NumHoles];
        int[] totalAttemptsPerHole = new int[NumHoles];

        foreach (Match match in Matches)
            foreach (ScoreCard scoreCard in match.ScoreCards)
                for (int i = 0; i < scoreCard.Strokes.Length; i++)
                {
                    totalStrokesPerHole[i] += scoreCard.Strokes[i];
                    totalAttemptsPerHole[i]++;
                }

        float[] averageStrokesPerHole = new float[NumHoles];
        for (int i = 0; i < totalStrokesPerHole.Length; i++)
            averageStrokesPerHole[i] = (float)totalStrokesPerHole[i] / totalAttemptsPerHole[i];

        return averageStrokesPerHole;
    }
    public int GetNumScoreCards()
    {
        int sum = 0;
        foreach (Match match in Matches)
            foreach (ScoreCard scoreCard in match.ScoreCards)
                sum++;

        return sum;
    }
    public int GetHolesWithScore(int score)
    {
        int sum = 0;
        foreach (Match match in Matches)
            foreach (ScoreCard scoreCard in match.ScoreCards)
                sum += scoreCard.GetHolesWithScore(score);
        return sum;
    }
    public float GetPercentHolesWithScore(int score)
    {
        return (float)GetHolesWithScore(score) / GetHolesCompleted() * 100;
    }
    public HoleInfo GetHardestHole(bool getHardest = true)
    {
        float[] averageStrokesPerHole = GetAverageStrokePerHole();

        HoleInfo bestHole = new HoleInfo() { AverageStrokes = getHardest ? float.MinValue : float.MaxValue, HoleNum = -1 };
        for (int i = 0; i < averageStrokesPerHole.Length; i++)
            if ((averageStrokesPerHole[i] > bestHole.AverageStrokes) == getHardest)
                bestHole = new HoleInfo() { AverageStrokes = averageStrokesPerHole[i], HoleNum = i + 1 };

        return bestHole;
    }
    public float GetCumulativeDifficulty()
    {
        float totalDifficulty = 0;
        int numReviews = 0;
        foreach (Review review in Reviews)
        {
            totalDifficulty += review.Difficulty;
            numReviews++;
        }

        return totalDifficulty / numReviews;
    }
    public float GetCumulativeRating()
    {
        float totalRating = 0;
        int numReviews = 0;
        foreach (Review review in Reviews)
        {
            totalRating += review.Rating;
            numReviews++;
        }

        return totalRating / numReviews;
    }
    public Review GetReview(uint golferID)
    {
        foreach (Review r in Reviews) if (r.GolferID == golferID) return r;
        return null;
    }
}
public class Match
{
    public DateTime Date { get; set; }
    public List<ScoreCard> ScoreCards { get; set; }
    public bool IsGolferPresent(Golfer golfer)
    {
        // If on one of the cards --> true
        foreach (ScoreCard card in ScoreCards)
            if (card.GolferID == golfer.ID)
                return true;

        // Else --> false
        return false;
    }
    public bool DidWin(Golfer g)
    {
        // Returns True if the Gofler passed in Won or Tied

        // Find golfers score in this game
        int totalScore = 0;
        foreach (ScoreCard card in ScoreCards)
            if (card.GolferID == g.ID)
                totalScore = card.GetTotalScore();

        // See if passed golfer has the highest score or Tied for it
        foreach (ScoreCard card in ScoreCards)
            if (card.GetTotalScore() < totalScore)
                return false;

        return true;
    }
    public ScoreCard GetScoreCard(uint golferID)
    {
        foreach (ScoreCard scorecard in ScoreCards) if (scorecard.GolferID == golferID) return scorecard;
        return null;
    }
}
public class ScoreCard
{
    public uint GolferID { get; set; }
    public int[] Strokes { get; set; } // Strokes for each hole

    public int GetTotalScore()
    {
        int sum = 0;
        foreach (int num in Strokes) sum += num;

        return sum;
    }
    // Gets the Number of holes with strokes in this score card
    public int GetHolesWithScore(int score)
    {
        int sum = 0;
        foreach (int strokes in Strokes) if (strokes == score) sum++;
        return sum;
    }
}
public class HoleInfo
{
    public int HoleNum { get; set; }
    public float AverageStrokes { get; set; }
    public Course Course { get; set; }
    public Loop Loop { get; set; }
    public string GetHoleString() { return HoleNum + (Loop != null ? ", " + Loop.GetLongName() : "") + (Course != null ? ", " + Course.GetLongName() : "") + " with " + AverageStrokes.ToString("n1") + " average strokes"; }
}
public class Review
{
    public uint GolferID { get; set; }
    public float Rating { get; set; }
    public float Difficulty { get; set; }
}
public enum State
{
    Alabama,
    Alaska,
    Arizona,
    Arkansas,
    California,
    Colorado,
    Connecticut,
    Delaware,
    Florida,
    Georgia,
    Hawaii,
    Idaho,
    Illinois,
    Indiana,
    Iowa,
    Kansas,
    Kentucky,
    Louisiana,
    Maine,
    Maryland,
    Massachusetts,
    Michigan,
    Minnesota,
    Mississippi,
    Missouri,
    Montana,
    Nebraska,
    Nevada,
    New_Hampshire,
    New_Jersey,
    New_Mexico,
    New_York,
    North_Carolina,
    North_Dakota,
    Ohio,
    Oklahoma,
    Oregon,
    Pennsylvania,
    Rhode_Island,
    South_Carolina,
    South_Dakota,
    Tennessee,
    Texas,
    Utah,
    Vermont,
    Virginia,
    Washington,
    West_Virginia,
    Wisconsin,
    Wyoming
}