
def is_letter(c):
    x = ord(c)
    return (ord('A') <= x <= ord('Z')) or (ord('a') <= x <= ord('z'))

def is_digit(c):
    x = ord(c)
    return ord('0') <= x <= ord('9')

def is_first_ident_char(c):
    return is_letter(c) or c == '_'

def is_next_ident_char(c):
    return is_letter(c) or is_digit(c) or c == '_'

def is_ident(s):
    assert isinstance(s, str)
    if len(s) < 1: return False
    c = s[0]
    if not is_first_ident_char(c): return False
    for c in s[1:]:
        if not is_next_ident_char(c): return False
    return True


