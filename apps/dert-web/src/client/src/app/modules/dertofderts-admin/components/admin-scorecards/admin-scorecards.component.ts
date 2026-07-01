import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScoreCard } from 'app/models/app/ScoreCard';
import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { DodUserResultsDto } from 'app/models/dto/DodUserResultsDto';

@Component({
  selector: 'app-admin-scorecards',
  templateUrl: './admin-scorecards.component.html',
  styleUrls: ['./admin-scorecards.component.scss']
})
export class AdminScorecardsComponent implements OnInit {

  public title: string;
  public icon: string;

  public scorecards: Array<ScoreCard> = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {

    if (this.activatedRoute.snapshot.data.cardsType === 'judge') {

      const resolved: DodUserResultsDto = this.activatedRoute.snapshot.data.scorecards;
      this.title = resolved.name;
      this.icon = 'people';
      this.scorecards = resolved.scoreCards;
    }

    if (this.activatedRoute.snapshot.data.cardsType === 'submission') {
      const resolved: DodGroupResultsDto = this.activatedRoute.snapshot.data.scorecards;
      this.title = resolved.groupName;
      this.icon = 'receipt';
      this.scorecards = resolved.scoreCards.map(sc => {
        return {
          dodResultId: sc.dodResultId,
          dodSubmissionId: sc.dodSubmissionId,
          scoreCategories: sc.scoreCategories,
          comments: sc.comments,
          isOfficial: sc.isOfficial
        };
      });
    }
  }

  public onScoreCardView(scoreCard: ScoreCard) {
    this.router.navigate([`/dertofderts/score/${scoreCard.dodSubmissionId}`]);
  }
}
