import UIManager from "./components/ui_manager";
import GameCanvas from "./components/game_canvas";
import getElementById from "./utils/dom";

const PRIMARY_UI_MANAGER_ID: string = "primary-ui-manager";
const PRIMARY_GAME_CANVAS_ID: string = "primary-game-canvas";

const CREATE_GAME_BTN_ID: string = "main-menu-create-game-btn";

const JOIN_GAME_INPUT_ID: string = "main-menu-join-game-input";
const JOIN_GAME_BTN_ID: string = "main-menu-join-game-btn";

const GAME_ID_DISPLAY_ID: string = "start-game-id";
const GAME_PLAYER_COUNT_DISPLAY_ID: string = "start-game-player-count";

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

function main(): void {
    const ui_manager: UIManager = getElementById(PRIMARY_UI_MANAGER_ID);
    const game_canvas: GameCanvas = getElementById(PRIMARY_GAME_CANVAS_ID);

    const create_game_btn: HTMLButtonElement = getElementById(CREATE_GAME_BTN_ID);

    const join_game_input: HTMLInputElement = getElementById(JOIN_GAME_INPUT_ID);
    const join_game_btn: HTMLButtonElement = getElementById(JOIN_GAME_BTN_ID);

    const game_id_display: HTMLElement = getElementById(GAME_ID_DISPLAY_ID);
    const game_player_count_display: HTMLElement = getElementById(GAME_PLAYER_COUNT_DISPLAY_ID);

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
}

document.addEventListener("DOMContentLoaded", main);