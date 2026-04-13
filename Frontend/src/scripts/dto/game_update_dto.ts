export interface TileUpdate {
    isRedTeam: boolean;
    strength: number;
    xIndex: number;
    yIndex: number;
}

export interface JoinUpdate {
    playerID: number;
    xPosition: number;
    yPosition: number;
    isLeaving: boolean;
    isRedTeam: boolean;
}

export interface PlayerUpdate {
    playerID: number;
    xPosition: number;
    yPosition: number;
}

export default interface GameUpdateDTO {
    gameTimeSecs: number;

    redTeamCoverage: number;
    blueTeamCoverage: number;

    tileUpdates: TileUpdate[];
    joinData: JoinUpdate[];
    playerUpdates: PlayerUpdate[];
}