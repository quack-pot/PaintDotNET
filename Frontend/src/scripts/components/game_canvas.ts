import * as THREE from "three";

const DEG2RAD: number = Math.PI / 180.0;

const CAMERA_HEIGHT: number = 6.0;
const CAMERA_Z_OFFSET: number = 4.0;

const CAMERA_LERP_SPEED: number = 5.0;

const CLEAR_COLOR: THREE.ColorRepresentation = 0xffffff;

const INITIAL_GRID_WIDTH: number = 64;
const INITIAL_GRID_HEIGHT: number = 64;

const TILE_SIZE: number = 1.0;
const PLAYER_SIZE: number = 0.5;

const GRID_HEIGHT: number = -0.5 * PLAYER_SIZE;

const PAINT_LEVEL_ZERO: THREE.ColorRepresentation = 0xd0d0d0;

const PAINT_LEVEL_ONE_RED: THREE.ColorRepresentation = 0xd99eae;
const PAINT_LEVEL_TWO_RED: THREE.ColorRepresentation = 0xa64c65;
const PAINT_LEVEL_MAX_RED: THREE.ColorRepresentation = 0x6b001f;

const PAINT_LEVEL_ONE_BLUE: THREE.ColorRepresentation = 0xae9ed9;
const PAINT_LEVEL_TWO_BLUE: THREE.ColorRepresentation = 0x654ca6;
const PAINT_LEVEL_MAX_BLUE: THREE.ColorRepresentation = 0x1f006b;

const RED_PLAYER_COLOR: THREE.ColorRepresentation = 0xff3333;
const BLUE_PLAYER_COLOR: THREE.ColorRepresentation = 0x3333ff;

export default class GameCanvas extends HTMLElement {
    private scene: THREE.Scene = new THREE.Scene();
    private camera: THREE.PerspectiveCamera = new THREE.PerspectiveCamera(
        70.0, // fov
        1.0, // aspect (set dynamically)
        0.1, // near-plane
        500.0 // far-plane
    );

    private renderer: THREE.WebGLRenderer = new THREE.WebGLRenderer({ antialias: true });

    private grid_width: number = 0;
    private grid_height: number = 0;

    private north_wall: THREE.Mesh = new THREE.Mesh(new THREE.BoxGeometry(), new THREE.MeshStandardMaterial({ color: PAINT_LEVEL_ZERO }))
    private south_wall: THREE.Mesh = new THREE.Mesh(new THREE.BoxGeometry(), new THREE.MeshStandardMaterial({ color: PAINT_LEVEL_ZERO }))
    private east_wall: THREE.Mesh = new THREE.Mesh(new THREE.BoxGeometry(), new THREE.MeshStandardMaterial({ color: PAINT_LEVEL_ZERO }))
    private west_wall: THREE.Mesh = new THREE.Mesh(new THREE.BoxGeometry(), new THREE.MeshStandardMaterial({ color: PAINT_LEVEL_ZERO }))

    private tile_grid: THREE.InstancedMesh = new THREE.InstancedMesh(undefined, undefined, 0);

    private player: THREE.Mesh = new THREE.Mesh(
        new THREE.BoxGeometry(PLAYER_SIZE, PLAYER_SIZE, PLAYER_SIZE),
        new THREE.MeshStandardMaterial({
            color: RED_PLAYER_COLOR,
        })
    );

    private other_players: THREE.Mesh[] = [];
    private player_id_to_index: Map<number, number> = new Map<number, number>()
    private player_index_to_id: Map<number, number> = new Map<number, number>()

    private last_frame_time: number = Date.now() * 0.001;

    constructor() {
        super();

        this.north_wall.position.setY(GRID_HEIGHT + 0.5);
        this.south_wall.position.setY(GRID_HEIGHT + 0.5);
        this.east_wall.position.setY(GRID_HEIGHT + 0.5);
        this.west_wall.position.setY(GRID_HEIGHT + 0.5);

        this.scene.add(this.north_wall, this.south_wall, this.east_wall, this.west_wall);

        this.resetGrid(INITIAL_GRID_WIDTH, INITIAL_GRID_HEIGHT);

        this.player.position.set(INITIAL_GRID_WIDTH * TILE_SIZE * 0.5, 0.0, INITIAL_GRID_HEIGHT * TILE_SIZE * 0.5);
        this.scene.add(this.player);

        this.camera.position.set(this.player.position.x, CAMERA_HEIGHT, this.player.position.z + CAMERA_Z_OFFSET);
        this.camera.rotation.set(-60.0 * DEG2RAD, 0.0, 0.0);

        this.scene.fog = new THREE.Fog(CLEAR_COLOR, 6.0, 12.0)

        const ambient_lighting: THREE.AmbientLight = new THREE.AmbientLight(0xffffff, 2.0);
        this.scene.add(ambient_lighting);

        const directional_light: THREE.DirectionalLight = new THREE.DirectionalLight(0xffffff, 1.0);
        this.scene.add(directional_light);

        this.renderer.setClearColor(CLEAR_COLOR);
        this.renderer.setAnimationLoop(this.onRender.bind(this));
        this.appendChild(this.renderer.domElement);

        this.onResize();
        window.addEventListener("resize", this.onResize.bind(this));
    }

    public addOtherPlayer(id: number, is_red_team: boolean, initial_x: number, initial_y: number): void {
        const existing_index: number | undefined = this.player_id_to_index.get(id);
        if (existing_index !== undefined) {
            const existing_player: THREE.Mesh = this.other_players[existing_index];

            existing_player.position.setX(initial_x * TILE_SIZE);
            existing_player.position.setZ(initial_y * TILE_SIZE);

            (existing_player.material as THREE.MeshStandardMaterial).color = new THREE.Color(
                is_red_team ? RED_PLAYER_COLOR : BLUE_PLAYER_COLOR
            );

            return;
        }

        const new_index: number = this.other_players.length;

        const new_player: THREE.Mesh = new THREE.Mesh(
            new THREE.BoxGeometry(PLAYER_SIZE, PLAYER_SIZE, PLAYER_SIZE),
            new THREE.MeshStandardMaterial({
                color: is_red_team ? RED_PLAYER_COLOR : BLUE_PLAYER_COLOR,
            })
        );

        new_player.position.setX(initial_x * TILE_SIZE);
        new_player.position.setZ(initial_y * TILE_SIZE);
        this.scene.add(new_player);

        this.other_players.push(new_player);
        this.player_id_to_index.set(id, new_index);
        this.player_index_to_id.set(new_index, id);
    }

    public removeOtherPlayer(id: number): void {
        const index: number | undefined = this.player_id_to_index.get(id);
        if (index === undefined) {
            return;
        }

        const last_index: number = this.other_players.length - 1;
        const last_id: number = this.player_index_to_id.get(index)!;

        this.player_id_to_index.delete(id);
        this.player_index_to_id.delete(last_index);

        this.scene.remove(this.other_players[index]);
        this.other_players[index] = this.other_players[last_index];
        this.other_players.pop();

        if (id === last_id) {
            return;
        }

        this.player_id_to_index.set(last_id, index);
        this.player_index_to_id.set(index, last_id);
    }

    public updateOtherPlayer(id: number, new_x: number, new_y: number): void {
        const index: number | undefined = this.player_id_to_index.get(id);
        if (index === undefined) {
            return;
        }

        const player: THREE.Mesh = this.other_players[index];
        player.position.setX(((new_x - 0.5) * TILE_SIZE));
        player.position.setZ(((new_y - 0.5) * TILE_SIZE));
    }

    public updatePlayerTeam(is_red_team: boolean): void {
        (this.player.material as THREE.MeshStandardMaterial).color = new THREE.Color(
            is_red_team ? RED_PLAYER_COLOR : BLUE_PLAYER_COLOR
        );
    }

    public updatePlayerPosition(x_position: number, y_position: number): void {
        this.player.position.setX((x_position - 0.5) * TILE_SIZE);
        this.player.position.setZ((y_position - 0.5) * TILE_SIZE);
    }

    public updateTile(x_idx: number, y_idx: number, is_red_team: boolean, strength: number): void {
        const idx: number = x_idx + (y_idx * this.grid_width);

        if (idx < 0 || idx >= this.grid_width * this.grid_height) {
            return;
        }

        switch (strength) {
            default: {
                this.tile_grid.setColorAt(idx, new THREE.Color(PAINT_LEVEL_ZERO));
                break;
            }

            case 1: {
                this.tile_grid.setColorAt(idx, new THREE.Color(
                    is_red_team ? PAINT_LEVEL_ONE_RED : PAINT_LEVEL_ONE_BLUE
                ));
                break;
            }

            case 2: {
                this.tile_grid.setColorAt(idx, new THREE.Color(
                    is_red_team ? PAINT_LEVEL_TWO_RED : PAINT_LEVEL_TWO_BLUE
                ));
                break;
            }

            case 3: {
                this.tile_grid.setColorAt(idx, new THREE.Color(
                    is_red_team ? PAINT_LEVEL_MAX_RED : PAINT_LEVEL_MAX_BLUE
                ));
                break;
            }
        }

        this.tile_grid.instanceColor!.needsUpdate = true;
    }

    public resetGrid(width: number, height: number): void {
        const total_tiles: number = width * height;

        if (width !== this.grid_width || height !== this.grid_height) {
            this.grid_width = width;
            this.grid_height = height;

            const world_width: number = TILE_SIZE * width;
            const world_height: number = TILE_SIZE * height;

            this.north_wall.scale.setX(world_width);
            this.north_wall.position.setX(0.5 * (world_width - TILE_SIZE));
            this.north_wall.position.setZ(-TILE_SIZE);

            this.west_wall.scale.setZ(world_height + 2.0 * TILE_SIZE);
            this.west_wall.position.setX(-TILE_SIZE);
            this.west_wall.position.setZ(0.5 * (world_height - TILE_SIZE));

            this.south_wall.scale.setX(this.north_wall.scale.x);
            this.south_wall.position.setX(this.north_wall.position.x);
            this.south_wall.position.setZ(world_height)

            this.east_wall.scale.setZ(this.west_wall.scale.z);
            this.east_wall.position.setX(world_width);
            this.east_wall.position.setZ(this.west_wall.position.z);

            this.scene.remove(this.tile_grid);

            this.tile_grid = new THREE.InstancedMesh(
                new THREE.PlaneGeometry(TILE_SIZE, TILE_SIZE, 1, 1),
                new THREE.MeshStandardMaterial({ color: 0xffffff }),
                total_tiles
            );

            this.scene.add(this.tile_grid);

            const dummy: THREE.Object3D = new THREE.Object3D();
            dummy.position.setY(GRID_HEIGHT);
            dummy.rotation.set(-90.0 * DEG2RAD, 0.0, 0.0);

            for (let grid_x: number = 0; grid_x < width; ++grid_x) {
                dummy.position.setX(grid_x * TILE_SIZE);
                for (let grid_y: number = 0; grid_y < height; ++grid_y) {
                    dummy.position.setZ(grid_y * TILE_SIZE);

                    dummy.updateMatrix();

                    const idx: number = grid_x + (grid_y * width);

                    this.tile_grid.setMatrixAt(idx, dummy.matrix);
                    this.tile_grid.setColorAt(idx, new THREE.Color(PAINT_LEVEL_ZERO));
                }
            }

            this.tile_grid.instanceMatrix.needsUpdate = true;
            this.tile_grid.instanceColor!.needsUpdate = true;
            return;
        }

        for (let idx: number = 0; idx < total_tiles; ++idx) {
            this.tile_grid.setColorAt(idx, new THREE.Color(PAINT_LEVEL_ZERO));
        }

        this.tile_grid.instanceMatrix.needsUpdate = true;
        this.tile_grid.instanceColor!.needsUpdate = true;
    }

    private onResize(): void {
        const width: number = this.clientWidth;
        const height: number = this.clientHeight;

        if (width === 0 || height === 0) {
            return;
        }

        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();

        this.renderer.setSize(width, height);
        this.renderer.setPixelRatio(window.devicePixelRatio);
    }

    private onRender(): void {
        const current_frame_time: number = Date.now() * 0.001;
        const delta_time: number = current_frame_time - this.last_frame_time;
        this.last_frame_time = current_frame_time;

        const camera_frame_lerp: number = Math.min(CAMERA_LERP_SPEED * delta_time, 1.0);
        this.camera.position.x += (this.player.position.x - this.camera.position.x) * camera_frame_lerp;
        this.camera.position.z += (this.player.position.z + CAMERA_Z_OFFSET - this.camera.position.z) * camera_frame_lerp;

        this.renderer.render(this.scene, this.camera);
    }
}

customElements.define("game-canvas", GameCanvas);