import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DodResultComplaintDto } from 'app/models/dto/DodResultComplaintDto';
import { SubscriptionLike } from 'rxjs';
import { Conductor } from '../../../services/dertofderts-admin.conductor';
import { Tracker } from '../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-dod-admin-complaints',
  templateUrl: './complaints.component.html',
  styleUrls: ['./complaints.component.scss']
})
export class ComplaintsComponent implements OnInit {

  private _subscriptions: SubscriptionLike[] = [];
  private _snackBarDuration: number = 1200;

  public complaints: Array<DodResultComplaintDto> = [];
  public showHandled: boolean = false;

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor,
    public snackBar: MatSnackBar
  ) { }

  public ngOnInit() {

    this._subscriptions.push(this._tracker.dodComplaints$.subscribe((complaints: DodResultComplaintDto[]) => {
      this.complaints = complaints;
    }));

    this._conductor.initComplaints(!this.showHandled);
  }

  /** When we verify a complaint then this will remove the complaint from scores and listings */
  public verifyComplaint(complaint: DodResultComplaintDto) {
    const subs = this._conductor.validateComplaint(complaint).subscribe(() => {
      subs.unsubscribe();
      this.snackBar.open('Complaint Validated', 'close', { duration: this._snackBarDuration });
    });
  }

  public rejectComplaint(complaint: DodResultComplaintDto) {
    const subs = this._conductor.rejectComplaint(complaint).subscribe(() => {
      subs.unsubscribe();
      this.snackBar.open('Complaint Rejected', 'close', { duration: this._snackBarDuration });
    });
  }

  public onToggleShowHandled() {
    this.refreshData();
  }

  public refreshData() {
    this._conductor.initComplaints(!this.showHandled, true);
  }
}
