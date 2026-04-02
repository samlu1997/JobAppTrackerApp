import { useState } from "react";

export default function AIAnalyser() {
    const [jobDescription, setJobDescription] = useState("");
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleAnalyse = async () => {
        if (!jobDescription.trim()) return;

        setLoading(true);
        setError(null);
        setResult(null);

        try {
            const response = await fetch("/api/ai/analyse", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ jobDescription })
            });

            if (!response.ok) throw new Error("Something went wrong");

            const data = await response.json();
            setResult(data);
        } catch (err) {
            setError("Failed to analyse job description. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: "800px", margin: "0 auto", padding: "2rem" }}>
            <h1>Job Description Analyser</h1>
            <p>Paste a job description below to get keyword suggestions and a cover letter draft.</p>

            <textarea
                rows={10}
                style={{ width: "100%", padding: "1rem", fontSize: "1rem" }}
                placeholder="Paste job description here..."
                value={jobDescription}
                onChange={(e) => setJobDescription(e.target.value)}
            />

            <button
                onClick={handleAnalyse}
                disabled={loading}
                style={{ marginTop: "1rem", padding: "0.75rem 2rem", fontSize: "1rem" }}
            >
                {loading ? "Analysing..." : "Analyse"}
            </button>

            {error && <p style={{ color: "red" }}>{error}</p>}

            {result && (
                <div style={{ marginTop: "2rem" }}>
                    <h2>Keywords</h2>
                    <ul>
                        {result.keywords.map((k, i) => <li key={i}>{k}</li>)}
                    </ul>

                    <h2>Skills to Highlight</h2>
                    <ul>
                        {result.skills.map((s, i) => <li key={i}>{s}</li>)}
                    </ul>

                    <h2>Cover Letter Draft</h2>
                    <p style={{ whiteSpace: "pre-wrap" }}>{result.coverLetter}</p>
                </div>
            )}
        </div>
    );
}