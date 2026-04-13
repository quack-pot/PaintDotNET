export default interface NewGameResultDTO {
    gameID: number;
    playerID: number;

    initialX: number;
    initialY: number;

    isRedTeam: boolean;
}