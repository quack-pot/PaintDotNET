import getElementById from "./utils/dom";
import UIManager from "./components/ui_manager";
import GameCanvas from "./components/game_canvas";
import SignalManager from "./server/signal_manager";
import ServerRequests from "./server/requests";
import { UIState } from "./types/ui_states";
import type NewGameResultDTO from "./dto/new_game_result_dto";
import type JoinGameResultDTO from "./dto/join_game_result_dto";

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

    let game_id: number = 0;
    let player_id: number = 0;

    create_game_btn.addEventListener("click", async () => {
        if (ui_manager.getState() !== UIState.MAIN_MENU) {
            return;
        }

        const create_response: Response = await ServerRequests.POST("/api/lobby/create");

        if (create_response.status === 200) {
            const create_result: NewGameResultDTO = await create_response.json();
            
            game_id = create_result.gameID;
            player_id = create_result.playerID;
            game_canvas.updatePlayerPosition(create_result.initialX, create_result.initialY);
            game_canvas.updatePlayerTeam(create_result.isRedTeam);

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
            game_canvas.updatePlayerPosition(create_result.initialX, create_result.initialY);
            game_canvas.updatePlayerTeam(create_result.isRedTeam);

            ui_manager.updateState(UIState.WAITING_ON_HOST);
        }
    });

    const signal_manager: SignalManager = new SignalManager("/game");

    

    await signal_manager.start();
}

document.addEventListener("DOMContentLoaded", main);