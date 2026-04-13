export default interface PlayerInputDTO {
    gameID: number;
    playerID: number;

    isUpPressed: boolean;
    isDownPressed: boolean;
    isLeftPressed: boolean;
    isRightPressed: boolean;
}