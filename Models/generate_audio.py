import sys
from gtts import gTTS

def generate_audio(text, language):
    try:
        tts = gTTS(text=text, lang=language)
        tts.save("output.wav")
    except Exception as e:
        print(f"خطأ أثناء توليد الصوت: {e}")
        sys.exit(1)

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("يرجى تمرير النص واللغة كمعاملين.")
        sys.exit(1)

    input_text = sys.argv[1]
    input_language = sys.argv[2]

    generate_audio(input_text, input_language)
