import React, { useContext } from 'react'
import ProductContext from '../context/ProductContext'
import { Link } from 'react-router-dom';
import { FaUserCircle } from 'react-icons/fa';
import { MdLogout } from 'react-icons/md';
import { MdDashboard } from 'react-icons/md';
import "./Header.css";

const Header = ({title}) => {
    const {user,logout} = useContext(ProductContext);
  return (
    <header className='Header'>
        <h1 className='header-title'>{title}</h1>
        <nav className='header-nav'>
            {!user && (
                <>
                <Link to='/login' className="header-link">Login</Link>
                <Link to='/signup' className="header-link">Signup</Link>
                </>
            )}

            {user && (
                <>
                <Link to={user.role==="Admin"?"/admin":"/user"}
                className="header-dashboard">
                    <MdDashboard size={22}/>
                    <span style={{marginLeft:"6px"}}>Dashboard</span>
                </Link>
                <span className='header-username'>
                    <FaUserCircle size={22} style={{marginRight:"6px"}}/>
                    {user.username}
                </span>

                <button onClick={logout} className='header-logout'>
                    <MdLogout size={22} style={{marginRight:"6px"}}/>
                    Logout
                </button>
                </>
            )}
        
        </nav>      
    </header>
  )
}

export default Header;
