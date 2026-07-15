export type PlayerType = 'Goalie' | 'Defence' | 'Offence';

export interface Player {
  id: number;
  name: string;
  weight: number;
  height: number;
  playerType: PlayerType | number;
  externalResourceId: number;
}

export interface Team {
  id: number;
  name: string;
  universeId: number;
  universeName: string;
  attackers: number;
  defenders: number;
  players: Player[];
}

export interface Universe {
  id: number;
  name: string;
  apiUrl: string;
}

export interface CreateTeamRequest {
  universeId: number;
  name: string;
  attackers: number;
  defenders: number;
}
