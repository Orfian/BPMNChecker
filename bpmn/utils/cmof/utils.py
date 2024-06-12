
import sys


def to_upper(c):
    x = ord(c)
    if ord('a') <= x <= ord('z'):
        return chr(x - ord('a') + ord('A'))
    return c


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

def is_int(s):
    assert isinstance(s, str)
    if len(s) < 1: return False
    for c in s:
        if not is_digit(c): return False
    return True




class Output(object):

    __slots__ = [
        'out',
        'indent',
        'start_line'
    ]

    def __init__(self, f):
        self.out = f
        self.indent = 0
        self.start_line = True

    def write(self, s):
        if self.start_line:
            self.start_line = False
            self.out.write(' ' * self.indent * 4)
        self.out.write(s)
        return self

    def nl(self):
        self.out.write('\n')
        self.start_line = True
        return self

    def inc(self):
        self.indent += 1

    def dec(self):
        assert (self.indent > 0)
        self.indent -= 1



class ErrOutput (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def syntax_error(self, msg):
        self.out.write('\n')
        self.out.write("SYNTAX ERROR: ")
        self.out.write (msg)
        self.out.write('\n\n')
        sys.exit(1)

    def error(self, msg):
        self.out.write('\n')
        self.out.write("ERROR: ")
        self.out.write (msg)
        self.out.write('\n\n')
        sys.exit(1)

    def warning(self, msg):
        self.out.write('\n')
        self.out.write("WARNING: ")
        self.out.write (msg)
        self.out.write('\n\n')


