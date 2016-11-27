import os, sys
sys.path.insert(0, os.path.join('build', 'pymake2'))

from pymake2 import *
from pymake2.template.csharp import csc

@default_target
@depends_on('compile', 'content', 'libs')
def all(conf):
    pass

@after_target('clean')
def clean_mgcb(conf):
    delete_dir('obj')

@target(conf=csc.conf)
def content(conf):
    create_dir(r'bin\Content')

    mgcb = [ '/compress',
             '/intermediateDir:obj',
             '/outputDir:' + conf.bindir + '\\Content\\',
             '/quiet' ]

    with open('content.mgcb', 'w') as f:
        for s in mgcb:
            f.write(s + '\n')

        for s in find_files(r'src\starburst\content', [ '*.fx', '*.jpg', '*.png', '*.spritefont' ]):
            f.write('/build:' + s + '\n')

        for s in find_files(r'src\starburst\content\sound\effects', [ '*.mp3', '*.wav' ]):
            f.write('/processor:SoundEffectProcessor' + '\n')
            f.write('/build:' + s + '\n')

        for s in find_files(r'src\starburst\content\sound\music', [ '*.mp3', '*.wav' ]):
            f.write('/processor:SongProcessor' + '\n')
            f.write('/build:' + s + '\n')

    run_program(r'build\MonoGame\MGCB.exe', [ '/@:content.mgcb' ])
    # Yes, MGCB is literally retarded.
    copy(r'bin\Content\src\starburst\content', r'bin\Content')
    delete_dir(r'bin\Content\src')

    copy('.', 'bin', '*map*.png')
    delete_file('content.mgcb')

@target(conf=csc.conf)
def libs(conf):
    copy(r'lib\MonoGame', conf.bindir, '*.dll')
    copy(r'lib\SharpDX', conf.bindir, '*.dll')

pymake2({ 'name': 'Starburst.exe',

          'flags': ['/define:GAMEPAD_VIBRATION',
                    '/nologo',
                    '/optimize',
                    '/target:exe',
                    '/platform:x64',
                    '/warn:4' ],

          'libdirs': csc.conf.libdirs + [ r'lib\MonoGame', r'lib\SharpDX' ],

          'libs': [ 'MonoGame.Framework.dll', 'SharpDX.dll', 'SharpDX.XInput.dll' ] })
