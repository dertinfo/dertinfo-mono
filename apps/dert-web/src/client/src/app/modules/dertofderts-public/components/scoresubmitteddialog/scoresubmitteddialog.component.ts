import { Component, OnInit } from '@angular/core';
import { MatLegacyDialogRef as MatDialogRef } from '@angular/material/legacy-dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-scoresubmitteddialog',
  templateUrl: './scoresubmitteddialog.component.html',
  styleUrls: ['./scoresubmitteddialog.component.scss']
})
export class ScoreSubmittedDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ScoreSubmittedDialogComponent>,
    private router: Router
    ) { }

  ngOnInit() {
  }

  public onGoToListClick() {
    this.dialogRef.close();
    this.router.navigate(['/dertofderts/judging']);
  }

}
