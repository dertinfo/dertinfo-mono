using DertInfo.Models.Database;
using DertInfo.Services.Guards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.ExtensionMethods
{
    public static class Dance_HideResultsIfNotPublished
    {
        public async static Task HideResultsIfNotPublished(this Dance dance)
        {
            Guard.IsNotNull(dance.Competition);

            if (!dance.Competition.ResultsPublished)
            {
                dance.Overrun = false;
                dance.MarkingSheets = new List<MarkingSheet>();
                dance.MarkingSheetImages = new List<MarkingSheetImage>();
                foreach (var score in dance.DanceScores)
                {
                    score.MarkGiven = 0;
                }
            }
        }
    }
}
