import UIManager from "./components/ui_manager";
import GameCanvas from "./components/game_canvas";

const PRIMARY_UI_MANAGER_ID: string = "primary-ui-manager";
const PRIMARY_GAME_CANVAS_ID: string = "primary-game-canvas";

function main(): void {
    const ui_manager: UIManager = document.getElementById(PRIMARY_UI_MANAGER_ID)! as UIManager;
    const game_canvas: GameCanvas = document.getElementById(PRIMARY_GAME_CANVAS_ID)! as GameCanvas;
}

document.addEventListener("DOMContentLoaded", main);