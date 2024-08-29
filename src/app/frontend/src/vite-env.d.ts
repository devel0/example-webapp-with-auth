/// <reference types="vite/client" />

// https://vitejs.dev/guide/env-and-mode

interface ImportMetaEnv {
    readonly VITE_GITCOMMIT: string
    readonly VITE_GITCOMMITDATE: string
    readonly VITE_SERVERNAME: string    
    // more env variables...
}

interface ImportMeta {
    readonly env: ImportMetaEnv
}
