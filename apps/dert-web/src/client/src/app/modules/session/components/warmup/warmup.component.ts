import { Component, OnInit } from '@angular/core';
import { WarmupService } from 'app/core/services/warmup.service';

/**
 * Warmup flow steps 2–4 — static "Warming up" UI with recovery.
 *
 * - If already warm (e.g. refresh on this URL) → continue to pending /dashboard.
 * - Else start giveItAKick (GET /api/status) then navigate when ready.
 * Never a permanent dead-end when sessionwarm is already set.
 */
@Component({
  selector: 'app-warmup',
  templateUrl: './warmup.component.html',
  styleUrls: ['./warmup.component.scss']
})
export class WarmupComponent implements OnInit {

  constructor(private warmupService: WarmupService) { }

  ngOnInit() {
    if (this.warmupService.isApiWarm()) {
      console.log('WarmupComponent — already warm, continuing to pending URL');
      this.warmupService.continueTo();
      return;
    }

    console.log('WarmupComponent — kicking API');
    this.warmupService.giveItAKick();
  }

}
