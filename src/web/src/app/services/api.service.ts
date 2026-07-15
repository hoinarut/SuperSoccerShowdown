import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateTeamRequest, Team, Universe } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl.replace(/\/$/, '');

  private url(path: string): string {
    return `${this.apiUrl}${path}`;
  }

  getTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.url('/teams'));
  }

  getUniverses(): Observable<Universe[]> {
    return this.http.get<Universe[]>(this.url('/universes'));
  }

  createTeam(request: CreateTeamRequest): Observable<Team> {
    return this.http.post<Team>(this.url('/teams'), request);
  }
}
