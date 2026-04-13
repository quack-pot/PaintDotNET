import type { JoinUpdate } from "./game_update_dto";

export default interface GameStartedDTO {
    gridWidth: number;
    gridHeight: number;

    playerInitialValues: JoinUpdate[];
}