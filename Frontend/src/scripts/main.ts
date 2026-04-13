import getElementById from "./utils/dom";
import UIManager from "./components/ui_manager";
import GameCanvas from "./components/game_canvas";
import SignalManager, { SignalName } from "./server/signal_manager";
import ServerRequests from "./server/requests";
import { UIState } from "./types/ui_states";
import type NewGameResultDTO from "./dto/new_game_result_dto";
import type JoinGameResultDTO from "./dto/join_game_result_dto";
import type StartGameDTO from "./dto/start_game_dto";
import type PlayerInputDTO from "./dto/player_input_dto";
import type GameOverDTO from "./dto/game_over_dto";
import type EndGameDTO from "./dto/end_game_dto";
import type GameUpdateDTO from "./dto/game_update_dto";
import type GameStartedDTO from "./dto/game_started_dto";

const PLAYER_INPUT_POLL_TIME_MS: number = 150;

const PRIMARY_UI_MANAGER_ID: string = "primary-ui-manager";
const PRIMARY_GAME_CANVAS_ID: string = "primary-game-canvas";

const CREATE_GAME_BTN_ID: string = "main-menu-create-game-btn";

const JOIN_GAME_INPUT_ID: string = "main-menu-join-game-input";
const JOIN_GAME_BTN_ID: string = "main-menu-join-game-btn";

const GAME_ID_DISPLAY_ID: string = "start-game-id";
const START_GAME_BTN_ID: string = "start-game-btn";

const GAME_OVER_TITLE_ID: string = "game-over-title";
const GAME_OVER_HOST_TITLE_ID: string = "game-over-host-title";

const GAME_OVER_RED_PERCENT_ID: string = "game-over-red-percent";
const GAME_OVER_HOST_RED_PERCENT_ID: string = "game-over-host-red-percent";
const GAME_OVER_BLUE_PERCENT_ID: string = "game-over-blue-percent";
const GAME_OVER_HOST_BLUE_PERCENT_ID: string = "game-over-host-blue-percent";
const GAME_OVER_NONE_PERCENT_ID: string = "game-over-none-percent";
const GAME_OVER_HOST_NONE_PERCENT_ID: string = "game-over-host-none-percent";

const LEAVE_GAME_BTN_ID: string = "game-over-leave-btn";

const PLAY_AGAIN_BTN_ID: string = "game-over-host-play-again-btn";
const END_GAME_BTN_ID: string = "game-over-host-end-btn";

const GAME_HUD_RED_PERCENT_ID: string = "game-hud-red-percent";
const GAME_HUD_BLUE_PERCENT_ID: string = "game-hud-blue-percent";
const GAME_HUD_TIME_ID: string = "game-hud-time";

async function main(): Promise<void> {
    const signal_manager: SignalManager = new SignalManager("/game");
    const ui_manager: UIManager = getElementById(PRIMARY_UI_MANAGER_ID);
    const game_canvas: GameCanvas = getElementById(PRIMARY_GAME_CANVAS_ID);

    const create_game_btn: HTMLButtonElement = getElementById(CREATE_GAME_BTN_ID);

    const join_game_input: HTMLInputElement = getElementById(JOIN_GAME_INPUT_ID);
    const join_game_btn: HTMLButtonElement = getElementById(JOIN_GAME_BTN_ID);

    const game_id_display: HTMLElement = getElementById(GAME_ID_DISPLAY_ID);
    const start_game_btn: HTMLButtonElement = getElementById(START_GAME_BTN_ID);

    const game_over_title: HTMLElement = getElementById(GAME_OVER_TITLE_ID);
    const game_over_title_host: HTMLElement = getElementById(GAME_OVER_HOST_TITLE_ID);

    const game_over_red_percent: HTMLElement = getElementById(GAME_OVER_RED_PERCENT_ID);
    const game_over_red_percent_host: HTMLElement = getElementById(GAME_OVER_HOST_RED_PERCENT_ID);
    const game_over_blue_percent: HTMLElement = getElementById(GAME_OVER_BLUE_PERCENT_ID);
    const game_over_blue_percent_host: HTMLElement = getElementById(GAME_OVER_HOST_BLUE_PERCENT_ID);
    const game_over_none_percent: HTMLElement = getElementById(GAME_OVER_NONE_PERCENT_ID);
    const game_over_none_percent_host: HTMLElement = getElementById(GAME_OVER_HOST_NONE_PERCENT_ID);

    const leave_game_btn: HTMLButtonElement = getElementById(LEAVE_GAME_BTN_ID);

    const play_again_btn: HTMLButtonElement = getElementById(PLAY_AGAIN_BTN_ID);
    const end_game_btn: HTMLButtonElement = getElementById(END_GAME_BTN_ID);

    const game_hud_red_percent: HTMLElement = getElementById(GAME_HUD_RED_PERCENT_ID);
    const game_hud_blue_percent: HTMLElement = getElementById(GAME_HUD_BLUE_PERCENT_ID);
    const game_hud_time: HTMLElement = getElementById(GAME_HUD_TIME_ID);

    /////////////////////////////////////////////////////////////////

    let is_host: boolean = false;
    let game_id: number = 0;
    let player_id: number = 0;

    let player_input: PlayerInputDTO = {
        gameID: game_id,
        playerID: player_id,

        isUpPressed: true,
        isDownPressed: false,
        isLeftPressed: false,
        isRightPressed: false
    }

    create_game_btn.addEventListener("click", async () => {
        if (ui_manager.getState() !== UIState.MAIN_MENU) {
            return;
        }

        const create_response: Response = await ServerRequests.POST("/api/lobby/create");

        if (create_response.status === 200) {
            const create_result: NewGameResultDTO = await create_response.json();
            
            game_id = create_result.gameID;
            player_id = create_result.playerID;
            is_host = true;

            game_canvas.updatePlayerPosition(create_result.initialX, create_result.initialY);
            game_canvas.updatePlayerTeam(create_result.isRedTeam);

            signal_manager.invokeSignal(SignalName.JOIN_GROUP, `GAME-${game_id}`);

            game_id_display.textContent = `Game ID: ${game_id}`
            ui_manager.updateState(UIState.START_GAME_MENU);
        }
    });

    join_game_btn.addEventListener("click", async () => {
        if (ui_manager.getState() !== UIState.MAIN_MENU) {
            return;
        }

        const join_game_id: number = join_game_input.valueAsNumber;
        const join_response: Response = await ServerRequests.PUT(`/api/lobby/join/${join_game_id}`);

        if (join_response.status === 200) {
            const create_result: JoinGameResultDTO = await join_response.json();
            
            game_id = join_game_id;
            player_id = create_result.playerID;
            is_host = false;

            game_canvas.updatePlayerPosition(create_result.initialX, create_result.initialY);
            game_canvas.updatePlayerTeam(create_result.isRedTeam);

            signal_manager.invokeSignal(SignalName.JOIN_GROUP, `GAME-${game_id}`);

            ui_manager.updateState(UIState.WAITING_ON_HOST);
        }
    });

    start_game_btn.addEventListener("click", async () => {
        if (!is_host || ui_manager.getState() !== UIState.START_GAME_MENU) {
            return;
        }

        const dto: StartGameDTO = {
            playerID: player_id
        }

        await ServerRequests.PUT(`/api/lobby/start/${game_id}`, dto);
    });

    leave_game_btn.addEventListener("click", () => {
        signal_manager.invokeSignal(SignalName.LEAVE_GROUP, `GAME-${game_id}`);
        ServerRequests.PUT(`/api/lobby/leave/${game_id}/${player_id}`);
        ui_manager.updateState(UIState.MAIN_MENU);
    });

    play_again_btn.addEventListener("click", async () => {
        if (!is_host || ui_manager.getState() !== UIState.GAME_OVER_HOST) {
            return;
        }

        const dto: StartGameDTO = {
            playerID: player_id
        }

        await ServerRequests.PUT(`/api/lobby/start/${game_id}`, dto);
    });

    end_game_btn.addEventListener("click", async () => {
        if (!is_host || ui_manager.getState() !== UIState.GAME_OVER_HOST) {
            return;
        }

        const dto: EndGameDTO = {
            playerID: player_id
        }

        await ServerRequests.DELETE(`/api/lobby/end/${game_id}`, dto);
    });

    /////////////////////////////////////////////////////////////////

    signal_manager.registerCallback(SignalName.GAME_STARTED, (dto: GameStartedDTO) => {
        ui_manager.updateState(UIState.GAME_HUD);

        player_input.gameID = game_id;
        player_input.playerID = player_id;

        game_canvas.resetGrid(dto.gridWidth, dto.gridHeight);
    });

    signal_manager.registerCallback(SignalName.GAME_UPDATE, (dto: GameUpdateDTO) => {
        if (ui_manager.getState() !== UIState.GAME_HUD) {
            return;
        }

        const mins: number = Math.floor((dto.gameTimeSecs % 3600) / 60);
        const secs: number = Math.floor(dto.gameTimeSecs % 60);
        game_hud_time.textContent = `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`

        game_hud_red_percent.textContent = `Red: ${Math.round(dto.redTeamCoverage * 100)}%`;
        game_hud_blue_percent.textContent = `Blue: ${Math.round(dto.blueTeamCoverage * 100)}%`;

        // TODO: Update the game's state
    });

    signal_manager.registerCallback(SignalName.GAME_OVER, (dto: GameOverDTO) => {
        if (ui_manager.getState() !== UIState.GAME_HUD) {
            return;
        }

        if (dto.redTeamCoverage > dto.blueTeamCoverage) {
            game_over_title.textContent = "Red Team Victory!";
        } else if (dto.redTeamCoverage < dto.blueTeamCoverage) {
            game_over_title.textContent = "Blue Team Victory!";
        } else {
            game_over_title.textContent = "Tie!"
        }

        game_over_title_host.textContent = game_over_title.textContent;

        const none_coverage: number = Math.max(0.0, 1.0 - (dto.redTeamCoverage + dto.blueTeamCoverage));

        game_over_red_percent.textContent = `Red: ${Math.round(dto.redTeamCoverage * 100)}%`
        game_over_red_percent_host.textContent = game_over_red_percent.textContent;
        game_over_blue_percent.textContent = `Blue: ${Math.round(dto.blueTeamCoverage * 100)}%`
        game_over_blue_percent_host.textContent = game_over_blue_percent.textContent;
        game_over_none_percent.textContent = `None: ${Math.round(none_coverage * 100)}%`
        game_over_none_percent_host.textContent = game_over_none_percent.textContent;
        
        ui_manager.updateState(is_host ? UIState.GAME_OVER_HOST : UIState.GAME_OVER_RESULT);
    });

    signal_manager.registerCallback(SignalName.GAME_END, () => {
        signal_manager.invokeSignal(SignalName.LEAVE_GROUP, `GAME-${game_id}`);
        ui_manager.updateState(UIState.MAIN_MENU);
    });

    document.addEventListener("keydown", (key_event: KeyboardEvent) => {
        switch (key_event.key) {
            case "ArrowUp":
            case 'w':
                player_input.isUpPressed = true;
                return;
            case "ArrowDown":
            case 's':
                player_input.isDownPressed = true;
                return;
            case "ArrowLeft":
            case 'a':
                player_input.isLeftPressed = true;
                return;
            case "ArrowRight":
            case 'd':
                player_input.isRightPressed = true;
                return;
        }
    });
    document.addEventListener("keyup", (key_event: KeyboardEvent) => {
        switch (key_event.key) {
            case "ArrowUp":
            case 'w':
                player_input.isUpPressed = false;
                return;
            case "ArrowDown":
            case 's':
                player_input.isDownPressed = false;
                return;
            case "ArrowLeft":
            case 'a':
                player_input.isLeftPressed = false;
                return;
            case "ArrowRight":
            case 'd':
                player_input.isRightPressed = false;
                return;
        }
    });

    setTimeout(() => {
        if (ui_manager.getState() !== UIState.GAME_HUD) {
            return;
        }

        signal_manager.invokeSignal(SignalName.PLAYER_INPUT, player_input);
    }, PLAYER_INPUT_POLL_TIME_MS);

    await signal_manager.start();
}

document.addEventListener("DOMContentLoaded", main);