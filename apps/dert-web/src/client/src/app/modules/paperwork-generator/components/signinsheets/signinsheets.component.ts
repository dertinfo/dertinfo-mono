import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ScoreSheetDto, SignInSheetDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { PaperworkGeneratorConductor } from '../../services/paperwork-generator.conductor';

@Component({
  selector: 'app-signinsheets',
  templateUrl: './signinsheets.component.html',
  styleUrls: ['./signinsheets.component.css']
})
export class SignInSheetsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private _eventId: number;

  public dataLoaded: boolean = false;
  public signInSheetData: Array<SignInSheetDto> = [];
  public paginatedSignInSheetData: Array<any> = [];
  public debugData: any = null;
  public showDebug: boolean = false;
  private readonly ROWS_PER_PAGE = 26; // Calculated based on A4 landscape height

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: PaperworkGeneratorConductor,

  ) { }

  ngOnInit() {

    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._eventId = +params['id']; // (+) converts string 'id' to a number
      this.loadPopulatedData();
    }));

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  private loadPopulatedData() {

    const subs = this._conductor.getSignInSheetPopulatedData(this._eventId).subscribe((data) => {

      subs.unsubscribe();
      this.dataLoaded = true;
      this.signInSheetData = data;
      this.paginatedSignInSheetData = this.paginateSignInSheetData(data);
      this.debugData = this.paginatedSignInSheetData;
      console.log('paginatedSignInSheetData (first 3):', this.paginatedSignInSheetData.slice(0,3));
    });
  }

  private paginateSignInSheetData(signInSheets: Array<SignInSheetDto>): Array<any> {
    const paginatedData: Array<any> = [];

    signInSheets.forEach(sheet => {
      const membersArray = sheet.memberAttendances || [];
      const totalMembers = membersArray.length;
      const totalPages = Math.max(1, Math.ceil(totalMembers / this.ROWS_PER_PAGE));

      for (let page = 0; page < totalPages; page++) {
        const startIdx = page * this.ROWS_PER_PAGE;
        const endIdx = startIdx + this.ROWS_PER_PAGE;
        const pageMembers = membersArray.slice(startIdx, endIdx);

        // Calculate spare rows to fill the page
        const spareRowsCount = Math.max(0, this.ROWS_PER_PAGE - pageMembers.length);
        const spareRows = Array(spareRowsCount).fill(null);

        // Provide safe fallbacks for properties that may not exist on the DTO
        const groupName = sheet['groupName'] || (sheet.registration ? `Group ${sheet.registration.groupId}` : '');
        const memberAttendanceCount = totalMembers;
        const teamAttendanceCount = sheet['teamAttendanceCount'] || 0;
        const groupMemberPinCode = sheet['groupMemberPinCode'] || '';

        paginatedData.push({
          event: sheet.event,
          groupName: groupName,
          memberAttendanceCount: memberAttendanceCount,
          teamAttendanceCount: teamAttendanceCount,
          groupMemberPinCode: groupMemberPinCode,
          memberAttendances: pageMembers,
          spareRows: spareRows,
          pageNumber: page + 1,
          totalPages: totalPages
        });
      }
    });

    return paginatedData;
  }
}
