import './App.css';
import { useAuth0 } from "@auth0/auth0-react";
import AIAnalyser from './AIAnalyser';

function App() {
    const { isAuthenticated, isLoading, loginWithRedirect, logout, user } = useAuth0();

    if (isLoading) return <div>Loading...</div>;

    if (!isAuthenticated) {
        return (
            <div style={{ textAlign: 'center', marginTop: '5rem' }}>
                <h1>Please Hire Me</h1>
                <p>Sign in to start tailoring your job applications</p>
                <button onClick={() => loginWithRedirect()}>
                    Sign In
                </button>
            </div>
        )
    }

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem' }}>
                <h2>Please Hire Me</h2>
                <div>
                    <span>Welcome, {user.name}</span>
                    <button
                        onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
                        style={{ marginLeft: '1rem' }}
                    >
                        Sign Out
                    </button>
                </div>
            </div>
            <AIAnalyser />
        </div>
    )
}

export default App
