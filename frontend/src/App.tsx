import './App.css'
import { CartProvider } from './context/CartContext'
import AdminProjectsPage from './pages/AdminProjectsPage'
import CartPage from './pages/CartPage'
import DonatePage from './pages/DonatePage'
import ProjectPage from './pages/ProjectPage'
import { BrowserRouter as Router, Routes, Route} from 'react-router-dom'

function App() {
  return (
    <>
    <CartProvider>
      <Router>
        <Routes>
          <Route path="/" element={<ProjectPage/>} />
          <Route path="/projects" element={<ProjectPage/>}/>
          <Route path="/donate/:projectName/:projectId" element={<DonatePage/>}/>
          <Route path='/cart' element={<CartPage/>}/>
          <Route path="/adminprojects" element={<AdminProjectsPage />} />
        </Routes>
      </Router>
    </CartProvider>

    </>
  )
}

export default App
