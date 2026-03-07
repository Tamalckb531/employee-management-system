import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from "react-router-dom"
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
    <div className="min-h-screen w-full bg-white relative overflow-hidden">
      {/* Noise Texture (Darker Dots) Background */}
      <div
        className="absolute inset-0 z-0"
        style={{
          background: "#ffffff",
          backgroundImage: `
            radial-gradient(circle at top center, rgba(59, 130, 246, 0.5),transparent 70%)
          `,
        }}
      />
        <div className=' relative'>
          <App />
        </div>
      </div>
      </BrowserRouter>
  </StrictMode>,
)
