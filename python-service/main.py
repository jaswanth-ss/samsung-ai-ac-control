from dotenv import load_dotenv
from groq import Groq
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os

load_dotenv()

app = FastAPI()

groq_api = os.getenv("GROQ_API_KEY")

if not groq_api:
    raise ValueError("GROQ_API_KEY is not set in the environment variables.")

client = Groq(api_key=groq_api)


@app.get("/")
async def read_root():
    return {"message": "ChillDude API is running!"}


class RecommendTemperature(BaseModel):
    location: str
    current_temperature: float
    weather_condition: str
    time: str


@app.post("/recommend-temperature")
async def recommend_temperature(data: RecommendTemperature):
    try:
        chat_completion = client.chat.completions.create(
            model="llama-3.3-70b-versatile",
            messages=[
                {
                    "role": "system",
                    "content": "You are an expert in recommending the AC temperature based on the current weather conditions and time of day. You need to recommend the ideal AC temperature and fan speed for a user based on the provided location, current temperature, weather condition, and time. Consider all these factors to provide a temperature comfortable for the user to sleep peacefully.The available fan speeds are 'auto', 'low', 'medium', 'high', 'turbo'. Return only the recommended temperature in Celsius and fan speed without any additional text or explanation. Always return temperature between 16 and 30 degrees celsius and always an interger value. You are given temperature in Celsius and you should return the recommended temperature in Celsius as well. The recommended temerature is for Indian Weather conditions. The output should be in the following format: 22 auto (where 22 is the recommended temperature and auto is the recommended fan speed).",
                },
                {
                    "role": "user",
                    "content": (
                        f"Location: {data.location}, "
                        f"Current Temperature: {data.current_temperature}, "
                        f"Weather Condition: {data.weather_condition}, "
                        f"Time: {data.time}. "
                        "What is the ideal AC temperature?"
                    ),
                },
            ],
        )

        recommended_temperature = (
            chat_completion.choices[0].message.content.strip()
        )

        return {
            "recommended_temperature": int(recommended_temperature)
        }

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))