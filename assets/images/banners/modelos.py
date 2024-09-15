from PIL import Image, ImageFont, ImageDraw, ImageOps, ImageChops
import io, textwrap, json
from unidecode import unidecode

async def textsize(text_string, font):
    # https://stackoverflow.com/a/46220683/9263761
    ascent, descent = font.getmetrics()

    text_width = font.getmask(text_string).getbbox()[2]
    text_height = font.getmask(text_string).getbbox()[3] + descent

    return (text_width, text_height)
"""
banner: imagem do banner
name: nome do usuario
description: sobre mim do usuario (str)
reps: rep (int)
avatar: imagem do avatar
"""
async def modelo_default(banner, usuario, description, reps, avatar, badges):
	#Draw & Definições
	fonte_maior = ImageFont.truetype("fontes/Roboto-Light.ttf",60)
	fonte_menor = ImageFont.truetype("fontes/Roboto-Light.ttf",22)
	fonte_grossa =ImageFont.truetype("fontes/Roboto-Medium.ttf",25)
	template_draw = ImageDraw.Draw(banner)
	
	#Colando o nome no banner
	xtxt, ytxt = await textsize(usuario.name, fonte_maior)
	print(f"Tam. Texto: x: {xtxt}, y: {ytxt}")
	if xtxt > 350:
		#Se continuar sendo menor mesmo após a redução
		xtxt, _ = await textsize(usuario.name, ImageFont.truetype("fontes/Roboto-Light.ttf",48))
		if xtxt > 325:
			template_draw.text((25,10),f"{usuario.name[:15]}",font=ImageFont.truetype("fontes/Roboto-Light.ttf",48), fill=(255,255,255))
		else:
			template_draw.text((25,10),f"{usuario.name}",font=ImageFont.truetype("fontes/Roboto-Light.ttf",48), fill=(255,255,255))
	
	else:
		template_draw.text((25,10),f"{usuario.name}",font=fonte_maior, fill=(255,255,255))

	#Descrição
	#NADA AQUI!!!!!!!!!!!
	#Colando as reps no banner
	template_draw.text((10,320),f"Likes: {reps}",font=fonte_grossa, fill=(255,255,255))
	#Colando o nível de like
	liketype = Image.open("gui/icons/liketype_1.png")
	liketype = liketype.resize((30,30))
	banner.paste(liketype,(219,315),liketype)
	
	#Colando o Avatar
	banner.paste(avatar, (30,95), avatar)

	#Verificando se está em uma casa para adcionar as badges
	flags_all = str(usuario.public_flags.all())
	if "<UserFlags.hypesquad_" in flags_all:
		if "<UserFlags.hypesquad_brilliance:" in flags_all:
			badges.append("brilliance")
		elif "<UserFlags.hypesquad_balance:" in flags_all:
			badges.append("balance")
		elif "<UserFlags.hypesquad_bravery:" in flags_all:
			badges.append("bravery")

	#Verificando se possui active developer
	if "active_developer" in flags_all:
		badges.append("active_developer")
	
	#Colando as badges (44)
	x, y = 403, 65
	for badge in badges:
		icon_badge = Image.open(f"gui/icons/badge_{badge}.png")
		icon_badge = icon_badge.resize((44,44))
		#Ver se é rgb ou rgba
		if icon_badge.mode == "RGBA":
			banner.paste(icon_badge,(x,y),icon_badge)
		else:
			banner.paste(icon_badge,(x,y))
		#Acrescentar para posicionar a badge
		x += 44+14
		
	
	return banner