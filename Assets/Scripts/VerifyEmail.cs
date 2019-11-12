public static class VerifyEmail
{
    public static bool ValidateEmail(string email) {
        string[] atCharacter;
        string[] dotCharacter;
        atCharacter = email.Split("@"[0]);
        if (atCharacter.Length == 2) {
            dotCharacter = atCharacter[1].Split("."[0]);
            if (dotCharacter.Length >= 2) {
                if (dotCharacter[dotCharacter.Length - 1].Length == 0) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
}
