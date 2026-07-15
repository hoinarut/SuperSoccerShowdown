import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { CreateTeamRequest, PlayerType, Team, Universe } from '../../models/api.models';

@Component({
  selector: 'app-home',
  imports: [ReactiveFormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  private readonly api = inject(ApiService);
  private readonly formBuilder = inject(FormBuilder);

  readonly teams = signal<Team[]>([]);
  readonly universes = signal<Universe[]>([]);
  readonly loading = signal(true);
  readonly modalOpen = signal(false);
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    universeId: [0, [Validators.required, Validators.min(1)]],
    attackers: [1, [Validators.required, Validators.min(0), Validators.max(4)]],
    defenders: [3, [Validators.required, Validators.min(0), Validators.max(4)]],
  });

  constructor() {
    this.loadTeams();
  }

  loadTeams(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getTeams().subscribe({
      next: (teams) => {
        this.teams.set(teams);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load teams.');
        this.loading.set(false);
      },
    });
  }

  openModal(): void {
    this.error.set(null);
    this.form.reset({ name: '', universeId: 0, attackers: 1, defenders: 3 });

    this.api.getUniverses().subscribe({
      next: (universes) => {
        this.universes.set(universes);
        const defaultUniverseId = universes[0]?.id ?? 0;
        this.form.patchValue({ universeId: defaultUniverseId });
        this.modalOpen.set(true);
      },
      error: () => this.error.set('Failed to load universes.'),
    });
  }

  closeModal(): void {
    if (!this.submitting()) {
      this.modalOpen.set(false);
    }
  }

  lineupTotal(): number {
    const { attackers, defenders } = this.form.getRawValue();
    return attackers + defenders;
  }

  lineupValid(): boolean {
    return this.lineupTotal() === 4;
  }

  submit(): void {
    if (this.form.invalid || !this.lineupValid()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const request: CreateTeamRequest = this.form.getRawValue();

    this.api.createTeam(request).subscribe({
      next: (team) => {
        this.teams.update((teams) => [...teams, team]);
        this.submitting.set(false);
        this.modalOpen.set(false);
      },
      error: (err) => {
        const message =
          typeof err.error === 'string'
            ? err.error
            : 'Failed to create team. Check the lineup and try again.';
        this.error.set(message);
        this.submitting.set(false);
      },
    });
  }

  roleLabel(role: PlayerType | number): string {
    const normalized = this.normalizeRole(role);

    switch (normalized) {
      case 'Goalie':
        return 'Goalie';
      case 'Defence':
        return 'Defender';
      case 'Offence':
        return 'Attacker';
      default:
        return normalized;
    }
  }

  normalizeRole(role: PlayerType | number): PlayerType | 'Unknown' {
    if (typeof role === 'number') {
      switch (role) {
        case 1:
          return 'Goalie';
        case 2:
          return 'Defence';
        case 3:
          return 'Offence';
        default:
          return 'Unknown';
      }
    }

    return role;
  }

  universeImage(universeName: string): string {
    switch (universeName.toLowerCase()) {
      case 'pokemon':
        return '/universes/pokemon.svg';
      case 'starwars':
        return '/universes/starwars.svg';
      default:
        return '/universes/default.svg';
    }
  }
}
