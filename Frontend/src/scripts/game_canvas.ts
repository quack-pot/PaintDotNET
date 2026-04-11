import * as THREE from "three";

export default class GameCanvas extends HTMLElement {
    private scene: THREE.Scene = new THREE.Scene();
    private camera: THREE.PerspectiveCamera = new THREE.PerspectiveCamera(
        70.0, // fov
        1.0, // aspect (set dynamically)
        0.1, // near-plane
        500.0 // far-plane
    );

    private renderer: THREE.WebGLRenderer = new THREE.WebGLRenderer({ antialias: true });

    constructor() {
        super();

        this.camera.position.setZ(10.0);
        this.scene.add(new THREE.Mesh(new THREE.SphereGeometry(), new THREE.MeshNormalMaterial()));

        this.renderer.setAnimationLoop(this.onRender.bind(this));
        this.appendChild(this.renderer.domElement);

        this.onResize();
        window.addEventListener("resize", this.onResize.bind(this));
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
        this.renderer.render(this.scene, this.camera);
    }
}

customElements.define("game-canvas", GameCanvas);