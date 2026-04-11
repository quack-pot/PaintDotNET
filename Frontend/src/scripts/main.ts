import GameCanvas from "./game_canvas";

const PRIMARY_GAME_CANVAS_ID: string = "primary-game-canvas";

function main(): void {
    const game_canvas: GameCanvas = document.getElementById(PRIMARY_GAME_CANVAS_ID)! as GameCanvas;
}

document.addEventListener("DOMContentLoaded", main);